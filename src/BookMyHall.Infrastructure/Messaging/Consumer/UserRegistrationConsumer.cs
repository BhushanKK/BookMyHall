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
    // ================================================================
    // RabbitMQ configuration
    // ================================================================

    private const string QueueName = "identity.user.registration";
    private const string RoutingKey = "identity.user.registered";

    // ================================================================
    // Email configuration
    // ================================================================

    private const string LogoContentId = "bookmyhall-logo";
    private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqOptions.Value;
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;
    private readonly EmailOptions _emailOptions = emailOptions.Value;
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

    private IConnection? _connection;

    private IChannel? _channel;

    // ================================================================
    // Execute Background Service
    // ================================================================

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            // ========================================================
            // 1. Create RabbitMQ connection
            // ========================================================

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.HostName,
                Port = _rabbitMqOptions.Port,
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password,
                VirtualHost = _rabbitMqOptions.VirtualHost
            };

            _connection =
                await factory.CreateConnectionAsync(
                    stoppingToken);

            logger.LogInformation(
                "RabbitMQ connection created successfully.");

            // ========================================================
            // 2. Create RabbitMQ channel
            // ========================================================

            _channel =
                await _connection.CreateChannelAsync(
                    cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ channel created successfully.");

            // ========================================================
            // 3. Declare exchange
            // ========================================================

            await _channel.ExchangeDeclareAsync(
                exchange: _rabbitMqOptions.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ exchange declared: {ExchangeName}",
                _rabbitMqOptions.ExchangeName);

            // ========================================================
            // 4. Declare queue
            // ========================================================

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ queue declared: {QueueName}",
                QueueName);

            // ========================================================
            // 5. Bind queue
            // ========================================================

            await _channel.QueueBindAsync(
                queue: QueueName,
                exchange: _rabbitMqOptions.ExchangeName,
                routingKey: RoutingKey,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ queue bound successfully.");

            // ========================================================
            // 6. Process one message at a time
            // ========================================================

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken);

            // ========================================================
            // 7. Create RabbitMQ consumer
            // ========================================================

            var consumer =
                new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                await ProcessMessageAsync(
                    eventArgs,
                    stoppingToken);
            };

            // ========================================================
            // 8. Start consuming
            // ========================================================

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            // ========================================================
            // 9. Consumer started
            // ========================================================

            logger.LogInformation(
                "================================================");

            logger.LogInformation(
                "UserRegistrationConsumer started successfully.");

            logger.LogInformation(
                "Queue      : {QueueName}",
                QueueName);

            logger.LogInformation(
                "RoutingKey : {RoutingKey}",
                RoutingKey);

            logger.LogInformation(
                "================================================");

            // ========================================================
            // 10. Keep background service alive
            // ========================================================

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "UserRegistrationConsumer cancellation requested.");
        }
        catch (Exception exception)
        {
            logger.LogCritical(
                exception,
                "UserRegistrationConsumer stopped unexpectedly.");

            throw;
        }
    }

    // ================================================================
    // Process RabbitMQ Message
    // ================================================================

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
            // ========================================================
            // 1. Read RabbitMQ message
            // ========================================================

            var body =
                eventArgs.Body.ToArray();

            var json =
                Encoding.UTF8.GetString(body);

            logger.LogDebug(
                "RabbitMQ message received: {Message}",
                json);

            // ========================================================
            // 2. Deserialize message
            // ========================================================

            var message =
                JsonSerializer.Deserialize<UserRegisteredMessage>(
                    json);

            if (message is null)
            {
                logger.LogWarning(
                    "Received invalid UserRegisteredMessage.");

                await _channel.BasicNackAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);

                return;
            }

            logger.LogInformation(
                "Processing registration message. " +
                "UserId: {UserId}, Email: {Email}",
                message.UserId,
                message.EmailAddress);

            // ========================================================
            // 3. Create DI scope
            // ========================================================

            using var scope =
                serviceScopeFactory.CreateScope();

            // ========================================================
            // 4. Resolve email template service
            // ========================================================

            var emailTemplateService =
                scope.ServiceProvider
                    .GetRequiredService<IEmailTemplateService>();

            // ========================================================
            // 5. Resolve email sender
            // ========================================================

            var emailSender =
                scope.ServiceProvider
                    .GetRequiredService<IEmailSender>();

            // ========================================================
            // 6. Build frontend base URL
            // ========================================================

            var baseUrl =
                _frontendOptions.BaseUrl.TrimEnd('/');

            // ========================================================
            // 7. Build verification URL
            // ========================================================

            var verificationUrl =
                $"{baseUrl}/verify-email" +
                $"?userId={Uri.EscapeDataString(message.UserId.ToString())}" +
                $"&token={Uri.EscapeDataString(message.VerificationToken)}";

            logger.LogDebug(
                "Verification URL created for UserId: {UserId}",
                message.UserId);

            // ========================================================
            // 8. Common template placeholders
            // ========================================================

            var placeholders =
                new Dictionary<string, string>
                {
                    ["UserName"] =
                        message.FullName,

                    ["VerificationLink"] =
                        verificationUrl,

                    ["ExpiryMinutes"] =
                        _emailOptions
                            .VerificationExpiryMinutes
                            .ToString(),

                    ["WebsiteUrl"] =
                        baseUrl,

                    ["CurrentYear"] =
                        DateTime.UtcNow.Year.ToString()
                };

            // ========================================================
            // 9. Resolve email logo
            // ========================================================

            /*
             * Example:
             *
             * ContentRootPath:
             * D:\Github\BookMyHall\src\BookMyHall.Api
             *
             * LogoPath:
             * www/images/logo.png
             *
             * Result:
             * D:\Github\BookMyHall\src\BookMyHall.Api
             * \www\images\logo.png
             */

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
                "Application ContentRootPath: {ContentRootPath}",
                _hostEnvironment.ContentRootPath);

            logger.LogInformation(
                "Configured logo path: {ConfiguredLogoPath}",
                _emailOptions.LogoPath);

            logger.LogInformation(
                "Resolved email logo path: {LogoPath}",
                logoPath);

            // ========================================================
            // 10. Validate logo
            // ========================================================

            if (!File.Exists(logoPath))
            {
                logger.LogError(
                    "BookMyHall logo was not found at: {LogoPath}",
                    logoPath);

                throw new FileNotFoundException(
                    "BookMyHall email logo was not found.",
                    logoPath);
            }

            logger.LogInformation(
                "BookMyHall email logo found successfully.");

            // ========================================================
            // 11. Create inline logo attachment
            // ========================================================

            var logoAttachment =
                new EmailAttachment
                {
                    FilePath = logoPath,
                    ContentId = LogoContentId
                };

            var inlineAttachments =
                new[]
                {
                    logoAttachment
                };

            // ========================================================
            // 12. Render VerifyEmail.html
            // ========================================================

            logger.LogInformation(
                "Rendering VerifyEmail.html for {Email}.",
                message.EmailAddress);

            var verificationHtml =
                await emailTemplateService.RenderAsync(
                    EmailTemplateConstants.VerifyEmail,
                    placeholders,
                    stoppingToken);

            // ========================================================
            // 13. Create verification email
            // ========================================================

            var verificationEmail =
                new EmailMessage
                {
                    To = message.EmailAddress,

                    Subject =
                        "Verify your BookMyHall account",

                    HtmlBody =
                        verificationHtml,

                    InlineAttachments =
                        inlineAttachments
                };

            // ========================================================
            // 14. Send verification email
            // ========================================================

            logger.LogInformation(
                "Sending verification email to {Email}.",
                message.EmailAddress);

            await emailSender.SendAsync(
                verificationEmail,
                stoppingToken);

            logger.LogInformation(
                "Verification email sent successfully to {Email}.",
                message.EmailAddress);

            // ========================================================
            // 15. Render Welcome.html
            // ========================================================

            logger.LogInformation(
                "Rendering Welcome.html for {Email}.",
                message.EmailAddress);

            var welcomeHtml =
                await emailTemplateService.RenderAsync(
                    EmailTemplateConstants.Welcome,
                    placeholders,
                    stoppingToken);

            // ========================================================
            // 16. Create welcome email
            // ========================================================

            var welcomeEmail =
                new EmailMessage
                {
                    To = message.EmailAddress,

                    Subject =
                        "Welcome to BookMyHall 🎉",

                    HtmlBody =
                        welcomeHtml,

                    InlineAttachments =
                        inlineAttachments
                };

            // ========================================================
            // 17. Send welcome email
            // ========================================================

            logger.LogInformation(
                "Sending welcome email to {Email}.",
                message.EmailAddress);

            await emailSender.SendAsync(
                welcomeEmail,
                stoppingToken);

            logger.LogInformation(
                "Welcome email sent successfully to {Email}.",
                message.EmailAddress);

            // ========================================================
            // 18. ACK RabbitMQ message
            // ========================================================

            await _channel.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken);

            // ========================================================
            // 19. Success logging
            // ========================================================

            logger.LogInformation(
                "================================================");

            logger.LogInformation(
                "User registration processing completed.");

            logger.LogInformation(
                "UserId : {UserId}",
                message.UserId);

            logger.LogInformation(
                "Email  : {Email}",
                message.EmailAddress);

            logger.LogInformation(
                "Verification email : SUCCESS");

            logger.LogInformation(
                "Welcome email      : SUCCESS");

            logger.LogInformation(
                "RabbitMQ message   : ACK");

            logger.LogInformation(
                "================================================");
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "User registration email processing cancelled.");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Failed to process registration email message.");

            // ========================================================
            // NACK message
            // ========================================================

            if (_channel is not null &&
                !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    /*
                     * Do not endlessly requeue the message.
                     *
                     * If the logo is missing or the email configuration
                     * is invalid, requeue:true will create an infinite
                     * retry loop.
                     *
                     * For now, reject the message.
                     *
                     * Later we can implement a proper RabbitMQ
                     * Dead Letter Queue + retry mechanism.
                     */

                    await _channel.BasicNackAsync(
                        deliveryTag: eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: stoppingToken);

                    logger.LogWarning(
                        "RabbitMQ registration message rejected after processing failure.");
                }
                catch (Exception nackException)
                {
                    logger.LogError(
                        nackException,
                        "Failed to NACK RabbitMQ message.");
                }
            }
        }
    }

    // ================================================================
    // Graceful Shutdown
    // ================================================================

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Stopping UserRegistrationConsumer.");

        // ============================================================
        // Close RabbitMQ channel
        // ============================================================

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

        // ============================================================
        // Close RabbitMQ connection
        // ============================================================

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

        // ============================================================
        // Stop BackgroundService
        // ============================================================

        await base.StopAsync(
            cancellationToken);
    }
}