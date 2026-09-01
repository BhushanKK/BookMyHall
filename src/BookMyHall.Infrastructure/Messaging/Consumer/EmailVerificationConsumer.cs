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

public sealed class EmailVerificationConsumer(
    IOptions<RabbitMqOptions> rabbitMqOptions,
    IOptions<FrontendOptions> frontendOptions,
    IOptions<EmailOptions> emailOptions,
    IServiceScopeFactory serviceScopeFactory,
    IHostEnvironment hostEnvironment,
    ILogger<EmailVerificationConsumer> logger)
    : BackgroundService
{
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
            logger.LogInformation(
                "Starting EmailVerificationConsumer. Environment: {EnvironmentName}",
                _hostEnvironment.EnvironmentName);

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.HostName,
                Port = _rabbitMqOptions.Port,
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password,
                VirtualHost = _rabbitMqOptions.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync(
                stoppingToken);

            logger.LogInformation(
                "RabbitMQ connection created successfully.");

            _channel = await _connection.CreateChannelAsync(
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ channel created successfully.");

            await _channel.ExchangeDeclareAsync(
                exchange: _rabbitMqOptions.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: RabbitMqKeys.EmailVerificationQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(
                queue: RabbitMqKeys.EmailVerificationQueueName,
                exchange: _rabbitMqOptions.ExchangeName,
                routingKey: RabbitMqKeys.EmailVerificationRoutingKey,
                cancellationToken: stoppingToken);

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken);

            var consumer =
                new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                await ProcessMessageAsync(
                    eventArgs,
                    stoppingToken);
            };

            await _channel.BasicConsumeAsync(
                queue: RabbitMqKeys.EmailVerificationQueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "EmailVerificationConsumer started successfully. " +
                "Queue: {QueueName}, RoutingKey: {RoutingKey}",
                RabbitMqKeys.EmailVerificationQueueName,
                RabbitMqKeys.EmailVerificationRoutingKey);

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "EmailVerificationConsumer cancellation requested.");
        }
        catch (Exception exception)
        {
            logger.LogCritical(
                exception,
                "EmailVerificationConsumer stopped unexpectedly.");

            throw;
        }
    }

    private async Task ProcessMessageAsync(
        BasicDeliverEventArgs eventArgs,
        CancellationToken stoppingToken)
    {
        if (_channel is null)
        {
            logger.LogError(
                "RabbitMQ channel is not available.");

            return;
        }

        try
        {
            var json =
                Encoding.UTF8.GetString(
                    eventArgs.Body.ToArray());

            logger.LogDebug(
                "RabbitMQ email verification message received: {Message}",
                json);

            var message =
                JsonSerializer.Deserialize<EmailVerificationRequestedMessage>(
                    json);

            if (message is null)
            {
                logger.LogWarning(
                    "Received invalid EmailVerificationRequestedMessage.");

                await _channel.BasicNackAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);

                return;
            }

            logger.LogInformation(
                "Processing verification email. UserId: {UserId}, Email: {Email}",
                message.UserId,
                message.EmailAddress);

            using var scope =
                serviceScopeFactory.CreateScope();

            var emailTemplateService =
                scope.ServiceProvider
                    .GetRequiredService<IEmailTemplateService>();

            var emailSender =
                scope.ServiceProvider
                    .GetRequiredService<IEmailSender>();

            var baseUrl =
                _frontendOptions.BaseUrl.TrimEnd('/');

            var verificationUrl =
                $"{baseUrl}/verify-email" +
                $"?userId={Uri.EscapeDataString(message.UserId.ToString())}" +
                $"&token={Uri.EscapeDataString(message.VerificationToken)}";

            var placeholders =
                new Dictionary<string, string>
                {
                    ["UserName"] = message.FullName,
                    ["VerificationLink"] = verificationUrl,
                    ["ExpiryMinutes"] =
                        message.ExpiryMinutes.ToString(),
                    ["WebsiteUrl"] = baseUrl,
                    ["CurrentYear"] =
                        DateTime.UtcNow.Year.ToString()
                };

            // --------------------------------------------------------
            // Resolve logo
            // --------------------------------------------------------

            var relativeLogoPath =
                _emailOptions.LogoPath
                    .Replace(
                        '/',
                        Path.DirectorySeparatorChar)
                    .Replace(
                        '\\',
                        Path.DirectorySeparatorChar);

            var logoPath =
                Path.Combine(
                    _hostEnvironment.ContentRootPath,
                    relativeLogoPath);

            logger.LogInformation(
                "Resolved email logo path: {LogoPath}",
                logoPath);

            if (!File.Exists(logoPath))
            {
                logger.LogError(
                    "BookMyHall logo was not found at: {LogoPath}",
                    logoPath);

                throw new FileNotFoundException(
                    "BookMyHall email logo was not found.",
                    logoPath);
            }

            var inlineAttachments =
                new[]
                {
                    new EmailAttachment
                    {
                        FilePath = logoPath,
                        ContentId = LogoContentId
                    }
                };

            // --------------------------------------------------------
            // Render verification email
            // --------------------------------------------------------

            logger.LogInformation(
                "Rendering VerifyEmail template for {Email}.",
                message.EmailAddress);

            var html =
                await emailTemplateService.RenderAsync(
                    EmailTemplateConstants.VerifyEmail,
                    placeholders,
                    stoppingToken);

            var email =
                new EmailMessage
                {
                    To = message.EmailAddress,

                    Subject =
                        "Verify your BookMyHall account",

                    HtmlBody = html,

                    InlineAttachments =
                        inlineAttachments
                };

            // --------------------------------------------------------
            // Send email
            // --------------------------------------------------------

            logger.LogInformation(
                "Sending verification email to {Email}.",
                message.EmailAddress);

            await emailSender.SendAsync(
                email,
                stoppingToken);

            logger.LogInformation(
                "Verification email sent successfully to {Email}.",
                message.EmailAddress);

            // --------------------------------------------------------
            // ACK
            // --------------------------------------------------------

            await _channel.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "Email verification message processed successfully. " +
                "UserId: {UserId}",
                message.UserId);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "Email verification processing cancelled.");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Failed to process email verification message.");

            if (_channel is not null &&
                !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _channel.BasicNackAsync(
                        deliveryTag: eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: stoppingToken);

                    logger.LogWarning(
                        "Email verification message rejected after processing failure.");
                }
                catch (Exception nackException)
                {
                    logger.LogError(
                        nackException,
                        "Failed to NACK email verification message.");
                }
            }
        }
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Stopping EmailVerificationConsumer.");

        if (_channel is not null)
        {
            try
            {
                await _channel.CloseAsync(
                    cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "Error while closing RabbitMQ channel.");
            }

            _channel = null;
        }

        if (_connection is not null)
        {
            try
            {
                await _connection.CloseAsync(
                    cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "Error while closing RabbitMQ connection.");
            }

            _connection = null;
        }

        await base.StopAsync(
            cancellationToken);
    }
}