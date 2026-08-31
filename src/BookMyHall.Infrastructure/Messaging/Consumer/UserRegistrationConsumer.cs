using System.Text;
using System.Text.Json;
using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Contracts.Messaging;
using BookMyHall.Infrastructure.Configuration;
using BookMyHall.Infrastructure.Options;
using BookMyHall.Shared.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BookMyHall.Infrastructure.Messaging.Consumers;

public sealed class UserRegistrationConsumer(
    IOptions<RabbitMqOptions> rabbitMqOptions,
    IOptions<FrontendOptions> frontendOptions,
    IOptions<EmailOptions> emailOptions,
    IServiceScopeFactory serviceScopeFactory,
    IHostEnvironment hostEnvironment,
    ILogger<UserRegistrationConsumer> logger)
    : BackgroundService
{
    private const string QueueName = "identity.user.registration";
    private const string RoutingKey = "identity.user.registered";
    private const string LogoContentId = "bookmyhall-logo";
    private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqOptions.Value;
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;
    private readonly EmailOptions _emailOptions = emailOptions.Value;
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Starting UserRegistrationConsumer. Environment: {EnvironmentName}", _hostEnvironment.EnvironmentName);
            logger.LogInformation("Configured Frontend BaseUrl: {BaseUrl}", _frontendOptions.BaseUrl);

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.HostName,
                Port = _rabbitMqOptions.Port,
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password,
                VirtualHost = _rabbitMqOptions.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync
            (
                stoppingToken
            );

            logger.LogInformation("RabbitMQ connection created successfully.");

            _channel = await _connection.CreateChannelAsync
            (
                cancellationToken: stoppingToken
            );

            logger.LogInformation("RabbitMQ channel created successfully.");

            await _channel.ExchangeDeclareAsync
            (
                exchange: _rabbitMqOptions.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken
            );

            await _channel.QueueDeclareAsync
            (
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken
            );

            await _channel.QueueBindAsync
            (
                queue: QueueName,
                exchange: _rabbitMqOptions.ExchangeName,
                routingKey: RoutingKey,
                cancellationToken: stoppingToken
            );

            await _channel.BasicQosAsync
            (
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                await ProcessMessageAsync
                (
                    eventArgs,
                    stoppingToken
                );
            };

            await _channel.BasicConsumeAsync
            (
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            logger.LogInformation("UserRegistrationConsumer started successfully. Queue: {QueueName}, RoutingKey: {RoutingKey}", QueueName, RoutingKey);

            await Task.Delay
            (
                Timeout.Infinite,
                stoppingToken
            );
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("UserRegistrationConsumer cancellation requested.");
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "UserRegistrationConsumer stopped unexpectedly.");
            throw;
        }
    }

    private async Task ProcessMessageAsync(BasicDeliverEventArgs eventArgs, CancellationToken stoppingToken)
    {
        if (_channel is null)
        {
            logger.LogError("RabbitMQ channel is not available.");
            return;
        }

        try
        {
            var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            logger.LogDebug("RabbitMQ message received: {Message}", json);
            var message = JsonSerializer.Deserialize<UserRegisteredMessage>
            (
                json
            );

            if (message is null)
            {
                logger.LogWarning("Received invalid UserRegisteredMessage.");

                await _channel.BasicNackAsync
                (
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken
                );

                return;
            }

            logger.LogInformation("Processing registration message. UserId: {UserId}, Email: {Email}", message.UserId, message.EmailAddress);

            using var scope = serviceScopeFactory.CreateScope();

            var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var baseUrl = _frontendOptions.BaseUrl.TrimEnd('/');

            logger.LogInformation("Environment: {EnvironmentName}, Frontend BaseUrl: {BaseUrl}", _hostEnvironment.EnvironmentName, baseUrl);

            var verificationUrl = $"{baseUrl}/verify-email?userId={Uri.EscapeDataString(message.UserId.ToString())}&token={Uri.EscapeDataString(message.VerificationToken)}";

            var placeholders = new Dictionary<string, string>
            {
                ["UserName"] = message.FullName,
                ["VerificationLink"] = verificationUrl,
                ["ExpiryMinutes"] = message.ExpiryMinutes.ToString(),
                ["WebsiteUrl"] = baseUrl,
                ["CurrentYear"] = DateTime.UtcNow.Year.ToString()
            };

            var relativeLogoPath = _emailOptions.LogoPath
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

            var logoPath = Path.Combine
            (
                _hostEnvironment.ContentRootPath,
                relativeLogoPath
            );

            logger.LogInformation("Application ContentRootPath: {ContentRootPath}", _hostEnvironment.ContentRootPath);
            logger.LogInformation("Configured logo path: {ConfiguredLogoPath}", _emailOptions.LogoPath);
            logger.LogInformation("Resolved email logo path: {LogoPath}", logoPath);

            if (!File.Exists(logoPath))
            {
                logger.LogError("BookMyHall logo was not found at: {LogoPath}", logoPath);

                throw new FileNotFoundException
                (
                    "BookMyHall email logo was not found.",
                    logoPath
                );
            }

            logger.LogInformation("BookMyHall email logo found successfully.");

            var inlineAttachments = new[]
            {
                new EmailAttachment
                {
                    FilePath = logoPath,
                    ContentId = LogoContentId
                }
            };

            logger.LogInformation("Rendering VerifyEmail.html for {Email}.", message.EmailAddress);

            var verificationHtml = await emailTemplateService.RenderAsync
            (
                EmailTemplateConstants.VerifyEmail,
                placeholders,
                stoppingToken
            );

            var verificationEmail = new EmailMessage
            {
                To = message.EmailAddress,
                Subject = "Verify your BookMyHall account",
                HtmlBody = verificationHtml,
                InlineAttachments = inlineAttachments
            };

            logger.LogInformation("Sending verification email to {Email}.", message.EmailAddress);

            await emailSender.SendAsync
            (
                verificationEmail,
                stoppingToken
            );

            logger.LogInformation("Verification email sent successfully to {Email}.", message.EmailAddress);

            logger.LogInformation("Rendering Welcome.html for {Email}.", message.EmailAddress);

            var welcomeHtml = await emailTemplateService.RenderAsync
            (
                EmailTemplateConstants.Welcome,
                placeholders,
                stoppingToken
            );

            var welcomeEmail = new EmailMessage
            {
                To = message.EmailAddress,
                Subject = "Welcome to BookMyHall 🎉",
                HtmlBody = welcomeHtml,
                InlineAttachments = inlineAttachments
            };

            logger.LogInformation("Sending welcome email to {Email}.", message.EmailAddress);

            await emailSender.SendAsync
            (
                welcomeEmail,
                stoppingToken
            );

            logger.LogInformation("Welcome email sent successfully to {Email}.", message.EmailAddress);

            await _channel.BasicAckAsync
            (
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken
            );

            logger.LogInformation("User registration processing completed successfully. UserId: {UserId}, Email: {Email}", message.UserId, message.EmailAddress);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("User registration email processing cancelled.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to process registration email message.");

            if (_channel is not null && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _channel.BasicNackAsync
                    (
                        deliveryTag: eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: stoppingToken
                    );

                    logger.LogWarning("RabbitMQ registration message rejected after processing failure.");
                }
                catch (Exception nackException)
                {
                    logger.LogError(nackException, "Failed to NACK RabbitMQ message.");
                }
            }
        }
    }

    public override async Task StopAsync
    (
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Stopping UserRegistrationConsumer.");

        if (_channel is not null)
        {
            try
            {
                await _channel.CloseAsync
                (
                    cancellationToken
                );
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Error while closing RabbitMQ channel.");
            }

            _channel = null;
        }

        if (_connection is not null)
        {
            try
            {
                await _connection.CloseAsync
                (
                    cancellationToken
                );
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Error while closing RabbitMQ connection.");
            }

            _connection = null;
        }

        await base.StopAsync
        (
            cancellationToken
        );
    }
}