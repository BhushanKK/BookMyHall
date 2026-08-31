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

public sealed class PasswordResetSuccessConsumer(
    IOptions<RabbitMqOptions> rabbitMqOptions,
    IOptions<FrontendOptions> frontendOptions,
    IOptions<EmailOptions> emailOptions,
    IServiceScopeFactory serviceScopeFactory,
    IHostEnvironment hostEnvironment,
    ILogger<PasswordResetSuccessConsumer> logger)
    : BackgroundService
{
    private const string LogoContentId = "bookmyhall-logo";
    private const int PasswordResetExpiryMinutes = 30;

    private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqOptions.Value;
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;
    private readonly EmailOptions _emailOptions = emailOptions.Value;
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

    private const string QueueName = RabbitMqKeys.PasswordResetSuccessQueueName;
    private const string RoutingKey = RabbitMqKeys.PasswordResetSuccessRoutingKey;

    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync
    (
        CancellationToken stoppingToken
    )
    {
        try
        {
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

            logger.LogInformation
            (
                "PasswordResetSuccessConsumer started successfully. Queue: {QueueName}, RoutingKey: {RoutingKey}",
                QueueName,
                RoutingKey
            );

            await Task.Delay
            (
                Timeout.Infinite,
                stoppingToken
            );
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("PasswordResetSuccessConsumer cancellation requested.");
        }
        catch (Exception exception)
        {
            logger.LogCritical
            (
                exception,
                "PasswordResetSuccessConsumer stopped unexpectedly."
            );

            throw;
        }
    }

    private async Task ProcessMessageAsync
    (
        BasicDeliverEventArgs eventArgs,
        CancellationToken stoppingToken
    )
    {
        if (_channel is null)
        {
            logger.LogError("RabbitMQ channel is not available.");
            return;
        }

        try
        {
            var body = eventArgs.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            logger.LogDebug("RabbitMQ message received: {Message}", json);

            var message = JsonSerializer.Deserialize<PasswordResetSuccessMessage>(json);

            if (message is null)
            {
                logger.LogWarning("Received invalid PasswordResetSuccessMessage.");

                await _channel.BasicNackAsync
                (
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken
                );

                return;
            }

            logger.LogInformation
            (
                "Processing password reset success message. UserId: {UserId}, Email: {Email}",
                message.UserId,
                message.EmailAddress
            );

            using var scope = serviceScopeFactory.CreateScope();

            var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var baseUrl = _frontendOptions.BaseUrl.TrimEnd('/');

            var placeholders = new Dictionary<string, string>
            {
                ["UserName"] = message.FullName,
                ["WebsiteUrl"] = baseUrl,
                ["ExpiryMinutes"] = PasswordResetExpiryMinutes.ToString(),
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

            logger.LogInformation
            (
                "Environment: {Environment}, Frontend BaseUrl: {BaseUrl}",
                _hostEnvironment.EnvironmentName,
                baseUrl
            );

            logger.LogInformation
            (
                "Resolved email logo path: {LogoPath}",
                logoPath
            );

            if (!File.Exists(logoPath))
            {
                logger.LogError("BookMyHall logo was not found at: {LogoPath}", logoPath);

                throw new FileNotFoundException
                (
                    "BookMyHall email logo was not found.",
                    logoPath
                );
            }

            var logoAttachment = new EmailAttachment
            {
                FilePath = logoPath,
                ContentId = LogoContentId
            };

            var inlineAttachments = new[]
            {
                logoAttachment
            };

            logger.LogInformation
            (
                "Rendering PasswordResetSuccess.html for {Email}.",
                message.EmailAddress
            );

            var passwordResetSuccessHtml = await emailTemplateService.RenderAsync
            (
                EmailTemplateConstants.PasswordResetSuccess,
                placeholders,
                stoppingToken
            );

            var passwordResetSuccessEmail = new EmailMessage
            {
                To = message.EmailAddress,
                Subject = "Your BookMyHall password was changed successfully",
                HtmlBody = passwordResetSuccessHtml,
                InlineAttachments = inlineAttachments
            };

            logger.LogInformation
            (
                "Sending password reset success email to {Email}.",
                message.EmailAddress
            );

            await emailSender.SendAsync
            (
                passwordResetSuccessEmail,
                stoppingToken
            );

            logger.LogInformation
            (
                "Password reset success email sent successfully to {Email}.",
                message.EmailAddress
            );

            await _channel.BasicAckAsync
            (
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken
            );
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Password reset success email processing cancelled.");
        }
        catch (Exception exception)
        {
            logger.LogError
            (
                exception,
                "Failed to process password reset success email message."
            );

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

                    logger.LogWarning
                    (
                        "RabbitMQ password reset success message rejected after processing failure."
                    );
                }
                catch (Exception nackException)
                {
                    logger.LogError
                    (
                        nackException,
                        "Failed to NACK password reset success message."
                    );
                }
            }
        }
    }

    public override async Task StopAsync
    (
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Stopping PasswordResetSuccessConsumer.");

        if (_channel is not null)
        {
            try
            {
                await _channel.CloseAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogWarning
                (
                    exception,
                    "Error while closing RabbitMQ channel."
                );
            }

            _channel = null;
        }

        if (_connection is not null)
        {
            try
            {
                await _connection.CloseAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogWarning
                (
                    exception,
                    "Error while closing RabbitMQ connection."
                );
            }

            _connection = null;
        }

        await base.StopAsync(cancellationToken);
    }
}