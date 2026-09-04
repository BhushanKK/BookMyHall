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
    private static readonly string QueueName = RabbitMqKeys.UserRegistrationQueueName;
    private static readonly string RoutingKey = RabbitMqKeys.UserRegistrationRoutingKey;
    private const string LogoContentId = "bookmyhall-logo";
    private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqOptions.Value;
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;
    private readonly EmailOptions _emailOptions = emailOptions.Value;
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
    private IConnection? _connection;
    private IChannel? _channel;
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation(
                "Starting UserRegistrationConsumer. Environment: {EnvironmentName}",
                _hostEnvironment.EnvironmentName);

            logger.LogInformation(
                "RabbitMQ configuration. Host: {Host}, Port: {Port}, VirtualHost: {VirtualHost}, Exchange: {Exchange}, Queue: {Queue}, RoutingKey: {RoutingKey}",
                _rabbitMqOptions.HostName,
                _rabbitMqOptions.Port,
                _rabbitMqOptions.VirtualHost,
                _rabbitMqOptions.ExchangeName,
                QueueName,
                RoutingKey);

            logger.LogInformation(
                "Configured Frontend BaseUrl: {BaseUrl}",
                _frontendOptions.BaseUrl);

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.HostName,
                Port = _rabbitMqOptions.Port,
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password,
                VirtualHost = _rabbitMqOptions.VirtualHost
            };

            // ============================================================
            // RabbitMQ Connection
            // ============================================================

            _connection = await factory.CreateConnectionAsync(
                stoppingToken);

            logger.LogInformation(
                "RabbitMQ connection created successfully. IsOpen: {IsOpen}",
                _connection.IsOpen);

            // ============================================================
            // RabbitMQ Channel
            // ============================================================

            _channel = await _connection.CreateChannelAsync(
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ channel created successfully. IsOpen: {IsOpen}",
                _channel.IsOpen);

            // ============================================================
            // Exchange
            // ============================================================

            await _channel.ExchangeDeclareAsync(
                exchange: _rabbitMqOptions.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ exchange declared successfully: {Exchange}",
                _rabbitMqOptions.ExchangeName);

            // ============================================================
            // Queue
            // ============================================================

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ queue declared successfully: {Queue}",
                QueueName);

            // ============================================================
            // Queue Binding
            // ============================================================

            await _channel.QueueBindAsync(
                queue: QueueName,
                exchange: _rabbitMqOptions.ExchangeName,
                routingKey: RoutingKey,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ queue binding created successfully. Queue: {Queue}, Exchange: {Exchange}, RoutingKey: {RoutingKey}",
                QueueName,
                _rabbitMqOptions.ExchangeName,
                RoutingKey);

            // ============================================================
            // QoS
            // ============================================================

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ QoS configured. PrefetchCount: 1");

            // ============================================================
            // Consumer
            // ============================================================

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                await ProcessMessageAsync(
                    eventArgs,
                    stoppingToken);
            };

            var consumerTag = await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ consumer registered successfully. ConsumerTag: {ConsumerTag}, Queue: {Queue}",
                consumerTag,
                QueueName);

            logger.LogInformation(
                "UserRegistrationConsumer started successfully.");

            // Keep BackgroundService alive.
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

        logger.LogInformation(
            "RabbitMQ message received. DeliveryTag: {DeliveryTag}, Queue: {Queue}",
            eventArgs.DeliveryTag,
            QueueName);

        try
        {
            // ============================================================
            // Deserialize Message
            // ============================================================

            var json = Encoding.UTF8.GetString(
                eventArgs.Body.ToArray());

            logger.LogDebug(
                "RabbitMQ message payload: {Message}",
                json);

            var message =
                JsonSerializer.Deserialize<UserRegisteredMessage>(json);

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
                "Processing registration message. UserId: {UserId}, Email: {Email}",
                message.UserId,
                message.EmailAddress);

            // ============================================================
            // Create Dependency Injection Scope
            // ============================================================

            using var scope =
                serviceScopeFactory.CreateScope();

            var emailTemplateService =
                scope.ServiceProvider
                    .GetRequiredService<IEmailTemplateService>();

            var emailSender =
                scope.ServiceProvider
                    .GetRequiredService<IEmailSender>();

            // ============================================================
            // Frontend URL
            // ============================================================

            var baseUrl =
                _frontendOptions.BaseUrl.TrimEnd('/');

            logger.LogInformation(
                "Frontend BaseUrl: {BaseUrl}",
                baseUrl);

            var verificationUrl =
                $"{baseUrl}/verify-email" +
                $"?userId={Uri.EscapeDataString(message.UserId.ToString())}" +
                $"&token={Uri.EscapeDataString(message.VerificationToken)}";

            logger.LogDebug(
                "Verification URL generated for UserId: {UserId}",
                message.UserId);

            // ============================================================
            // Email Placeholders
            // ============================================================

            var placeholders = new Dictionary<string, string>
            {
                ["UserName"] = message.FullName,
                ["VerificationLink"] = verificationUrl,
                ["ExpiryMinutes"] = message.ExpiryMinutes.ToString(),
                ["WebsiteUrl"] = baseUrl,
                ["CurrentYear"] = DateTime.UtcNow.Year.ToString()
            };

            // ============================================================
            // Logo
            // ============================================================

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

            var inlineAttachments = new[]
            {
                new EmailAttachment
                {
                    FilePath = logoPath,
                    ContentId = LogoContentId
                }
            };

            // ============================================================
            // Verification Email
            // ============================================================

            logger.LogInformation(
                "Rendering VerifyEmail.html for {Email}.",
                message.EmailAddress);

            var verificationHtml =
                await emailTemplateService.RenderAsync(
                    EmailTemplateConstants.VerifyEmail,
                    placeholders,
                    stoppingToken);

            var verificationEmail = new EmailMessage
            {
                To = message.EmailAddress,
                Subject = "Verify your BookMyHall account",
                HtmlBody = verificationHtml,
                InlineAttachments = inlineAttachments
            };

            logger.LogInformation(
                "Sending verification email to {Email}.",
                message.EmailAddress);

            await emailSender.SendAsync(
                verificationEmail,
                stoppingToken);

            logger.LogInformation(
                "Verification email sent successfully to {Email}.",
                message.EmailAddress);

            // ============================================================
            // Welcome Email
            // ============================================================

            logger.LogInformation(
                "Rendering Welcome.html for {Email}.",
                message.EmailAddress);

            var welcomeHtml =
                await emailTemplateService.RenderAsync(
                    EmailTemplateConstants.Welcome,
                    placeholders,
                    stoppingToken);

            var welcomeEmail = new EmailMessage
            {
                To = message.EmailAddress,
                Subject = "Welcome to BookMyHall 🎉",
                HtmlBody = welcomeHtml,
                InlineAttachments = inlineAttachments
            };

            logger.LogInformation(
                "Sending welcome email to {Email}.",
                message.EmailAddress);

            await emailSender.SendAsync(
                welcomeEmail,
                stoppingToken);

            logger.LogInformation(
                "Welcome email sent successfully to {Email}.",
                message.EmailAddress);

            // ============================================================
            // ACK
            // ============================================================

            await _channel.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ message ACKed successfully. DeliveryTag: {DeliveryTag}, UserId: {UserId}, Email: {Email}",
                eventArgs.DeliveryTag,
                message.UserId,
                message.EmailAddress);
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
                "Failed to process registration email message. DeliveryTag: {DeliveryTag}",
                eventArgs.DeliveryTag);

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
                        "RabbitMQ registration message rejected. DeliveryTag: {DeliveryTag}",
                        eventArgs.DeliveryTag);
                }
                catch (Exception nackException)
                {
                    logger.LogError(
                        nackException,
                        "Failed to NACK RabbitMQ message. DeliveryTag: {DeliveryTag}",
                        eventArgs.DeliveryTag);
                }
            }
        }
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Stopping UserRegistrationConsumer.");

        if (_channel is not null)
        {
            try
            {
                if (_channel.IsOpen)
                {
                    await _channel.CloseAsync(
                        cancellationToken);
                }
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
                if (_connection.IsOpen)
                {
                    await _connection.CloseAsync(
                        cancellationToken);
                }
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