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

public sealed class EmailVerifiedConsumer(
    IOptions<RabbitMqOptions> rabbitMqOptions,
    IOptions<FrontendOptions> frontendOptions,
    IOptions<EmailOptions> emailOptions,
    IServiceScopeFactory serviceScopeFactory,
    IHostEnvironment hostEnvironment,
    ILogger<EmailVerifiedConsumer> logger)
    : BackgroundService
{
    // ------------------------------------------------------------
    // RabbitMQ Configuration
    // ------------------------------------------------------------

    private const string QueueName = "identity.user.email-verified";
    private const string RoutingKey = "identity.user.email-verified";
    private const string LogoContentId = "bookmyhall-logo";
    private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqOptions.Value;
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;
    private readonly EmailOptions _emailOptions = emailOptions.Value;
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
    private IConnection? _connection;
    private IChannel? _channel;


    // ============================================================
    // ExecuteAsync
    // ============================================================

    protected override async Task ExecuteAsync( CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation(
                "Starting EmailVerifiedConsumer. Environment: {EnvironmentName}",
                _hostEnvironment.EnvironmentName);

            logger.LogInformation(
                "Configured Frontend BaseUrl: {BaseUrl}",
                _frontendOptions.BaseUrl);


            // --------------------------------------------------------
            // 1. RabbitMQ Connection
            // --------------------------------------------------------

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.HostName,
                Port = _rabbitMqOptions.Port,
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password,
                VirtualHost = _rabbitMqOptions.VirtualHost
            };


            _connection = await factory.CreateConnectionAsync(stoppingToken);

            logger.LogInformation("RabbitMQ connection created successfully for EmailVerifiedConsumer.");


            // --------------------------------------------------------
            // 2. RabbitMQ Channel
            // --------------------------------------------------------

            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            logger.LogInformation("RabbitMQ channel created successfully for EmailVerifiedConsumer.");

            // --------------------------------------------------------
            // 3. Exchange
            // --------------------------------------------------------

            await _channel.ExchangeDeclareAsync(
                exchange: _rabbitMqOptions.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);


            // --------------------------------------------------------
            // 4. Queue
            // --------------------------------------------------------

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);


            // --------------------------------------------------------
            // 5. Queue Binding
            // --------------------------------------------------------

            await _channel.QueueBindAsync(
                queue: QueueName,
                exchange: _rabbitMqOptions.ExchangeName,
                routingKey: RoutingKey,
                cancellationToken: stoppingToken);


            // --------------------------------------------------------
            // 6. QoS
            // --------------------------------------------------------

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken);


            // --------------------------------------------------------
            // 7. Consumer
            // --------------------------------------------------------

            var consumer =
                new AsyncEventingBasicConsumer(_channel);


            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                await ProcessMessageAsync(
                    eventArgs,
                    stoppingToken);
            };


            // --------------------------------------------------------
            // 8. Start Consuming
            // --------------------------------------------------------

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);


            logger.LogInformation(
                "EmailVerifiedConsumer started successfully. Queue: {QueueName}, RoutingKey: {RoutingKey}",
                QueueName,
                RoutingKey);


            // Keep BackgroundService alive

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "EmailVerifiedConsumer cancellation requested.");
        }
        catch (Exception exception)
        {
            logger.LogCritical(
                exception,
                "EmailVerifiedConsumer stopped unexpectedly.");

            throw;
        }
    }


    // ============================================================
    // Process Message
    // ============================================================

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
            // --------------------------------------------------------
            // 1. Read message
            // --------------------------------------------------------

            var json =
                Encoding.UTF8.GetString(
                    eventArgs.Body.ToArray());


            logger.LogDebug(
                "RabbitMQ EmailVerifiedMessage received: {Message}",
                json);


            // --------------------------------------------------------
            // 2. Deserialize message
            // --------------------------------------------------------

            var message =
                JsonSerializer.Deserialize<EmailVerifiedMessage>(
                    json);


            if (message is null)
            {
                logger.LogWarning(
                    "Received invalid EmailVerifiedMessage.");


                await _channel.BasicNackAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);

                return;
            }


            logger.LogInformation(
                "Processing email verified event. UserId: {UserId}, Email: {Email}",
                message.UserId,
                message.EmailAddress);


            // --------------------------------------------------------
            // 3. Create DI scope
            // --------------------------------------------------------

            using var scope =
                serviceScopeFactory.CreateScope();


            var emailTemplateService =
                scope.ServiceProvider
                    .GetRequiredService<IEmailTemplateService>();


            var emailSender =
                scope.ServiceProvider
                    .GetRequiredService<IEmailSender>();


            // --------------------------------------------------------
            // 4. Frontend URL
            // --------------------------------------------------------

            var baseUrl =
                _frontendOptions.BaseUrl.TrimEnd('/');


            logger.LogInformation(
                "Frontend BaseUrl: {BaseUrl}",
                baseUrl);


            // --------------------------------------------------------
            // 5. Build Password Setup URL
            // --------------------------------------------------------
            //
            // IMPORTANT:
            //
            // This assumes your frontend route is:
            //
            // /set-password?userId=...
            //
            // If your actual route is different,
            // change it here.
            //
            // --------------------------------------------------------

            var passwordSetupUrl =
                $"{baseUrl}/set-password" +
                $"?userId={Uri.EscapeDataString(message.UserId.ToString())}";


            // --------------------------------------------------------
            // 6. Email Placeholders
            // --------------------------------------------------------

            var placeholders =
                new Dictionary<string, string>
                {
                    ["UserName"] =
                        message.FullName,

                    ["PasswordSetupLink"] =
                        passwordSetupUrl,

                    ["WebsiteUrl"] =
                        baseUrl,

                    ["CurrentYear"] =
                        DateTime.UtcNow.Year.ToString()
                };


            // --------------------------------------------------------
            // 7. Resolve Logo
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


            // --------------------------------------------------------
            // 8. Inline Logo
            // --------------------------------------------------------

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
            // 9. Render Email Template
            // --------------------------------------------------------

            logger.LogInformation(
                "Rendering EmailVerified.html for {Email}.",
                message.EmailAddress);


            var emailHtml =
                await emailTemplateService.RenderAsync(
                    EmailTemplateConstants.EmailVerified,
                    placeholders,
                    stoppingToken);


            // --------------------------------------------------------
            // 10. Create Email
            // --------------------------------------------------------

            var email =
                new EmailMessage
                {
                    To = message.EmailAddress,

                    Subject =
                        "Email verified successfully - BookMyHall",

                    HtmlBody = emailHtml,

                    InlineAttachments =
                        inlineAttachments
                };


            // --------------------------------------------------------
            // 11. Send Email
            // --------------------------------------------------------

            logger.LogInformation(
                "Sending email verified notification to {Email}.",
                message.EmailAddress);


            await emailSender.SendAsync(
                email,
                stoppingToken);


            logger.LogInformation(
                "Email verified notification sent successfully to {Email}.",
                message.EmailAddress);


            // --------------------------------------------------------
            // 12. ACK
            // --------------------------------------------------------

            await _channel.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken);


            logger.LogInformation(
                "EmailVerifiedMessage processed successfully. UserId: {UserId}",
                message.UserId);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "Email verified email processing cancelled.");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Failed to process EmailVerifiedMessage.");


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
                        "RabbitMQ EmailVerifiedMessage rejected after processing failure.");
                }
                catch (Exception nackException)
                {
                    logger.LogError(
                        nackException,
                        "Failed to NACK EmailVerifiedMessage.");
                }
            }
        }
    }


    // ============================================================
    // StopAsync
    // ============================================================

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Stopping EmailVerifiedConsumer.");


        // --------------------------------------------------------
        // Close Channel
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // Close Connection
        // --------------------------------------------------------

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