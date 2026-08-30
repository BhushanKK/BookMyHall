using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Contracts.Messaging;

namespace BookMyHall.Infrastructure.Messaging.Consumers;

public sealed class UserRegistrationConsumer(
    IOptions<RabbitMqOptions> options,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<UserRegistrationConsumer> logger)
    : BackgroundService
{
    private const string QueueName ="identity.user.registration";
    private const string RoutingKey = "identity.user.registered";
    private readonly RabbitMqOptions _options = options.Value;
    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // ========================================================
            // 1. Create RabbitMQ connection
            // ========================================================

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);

            logger.LogInformation("RabbitMQ connection created successfully.");

            // ========================================================
            // 2. Create RabbitMQ channel
            // ========================================================

            _channel =await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            logger.LogInformation("RabbitMQ channel created successfully.");

            // ========================================================
            // 3. Declare exchange
            // ========================================================

            await _channel.ExchangeDeclareAsync
            (
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken
            );

            // ========================================================
            // 4. Declare queue
            // ========================================================

            await _channel.QueueDeclareAsync
            (
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken
            );

            // ========================================================
            // 5. Bind queue to exchange
            // ========================================================

            await _channel.QueueBindAsync
            (
                queue: QueueName,
                exchange: _options.ExchangeName,
                routingKey: RoutingKey,
                cancellationToken: stoppingToken
            );

            // ========================================================
            // 6. Process one message at a time
            // ========================================================

            await _channel.BasicQosAsync
            (
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken
            );

            // ========================================================
            // 7. Create RabbitMQ consumer
            // ========================================================

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                await ProcessMessageAsync(eventArgs, stoppingToken);
            };

            // ========================================================
            // 8. Start consuming
            // ========================================================

            await _channel.BasicConsumeAsync
            (
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            logger.LogInformation("================================================");

            logger.LogInformation("UserRegistrationConsumer started successfully.");

            logger.LogInformation("Queue      : {QueueName}", QueueName);

            logger.LogInformation("RoutingKey : {RoutingKey}", RoutingKey);

            logger.LogInformation("================================================");

            // ========================================================
            // 9. Keep BackgroundService alive
            // ========================================================

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)  when (stoppingToken.IsCancellationRequested)
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
            // ========================================================
            // 1. Read RabbitMQ message
            // ========================================================

            var body = eventArgs.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            logger.LogDebug("RabbitMQ message received: {Message}", json);

            // ========================================================
            // 2. Deserialize message
            // ========================================================

            var message = JsonSerializer.Deserialize<UserRegisteredMessage>(json);

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

            logger.LogInformation
            (
                "Processing registration message. " +
                "UserId: {UserId}, Email: {Email}",
                message.UserId,
                message.EmailAddress
            );

            // ========================================================
            // 3. Create DI scope
            // ========================================================

            using var scope = serviceScopeFactory.CreateScope();

            // ========================================================
            // 4. Resolve email template service
            // ========================================================

            var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

            // ========================================================
            // 5. Resolve email sender
            // ========================================================

            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            // ========================================================
            // 6. Build verification URL
            // ========================================================

            var verificationUrl =
                $"https://bookmyhall.sycits.co.in/verify-email" +
                $"?userId={message.UserId}" +
                $"&token={Uri.EscapeDataString(message.VerificationToken)}";

            logger.LogDebug("Verification URL created for UserId: {UserId}", message.UserId);

            // ========================================================
            // 7. Common placeholders
            // ========================================================

            var placeholders = new Dictionary<string, string>
            {
                ["FullName"] = message.FullName,
                ["VerificationUrl"] = verificationUrl
            };

            // ========================================================
            // 8. Send Welcome Email
            // ========================================================
            //
            // Template:
            //
            // Email/Templates/Welcome.html
            //
            // RenderAsync receives "Welcome" because the
            // EmailTemplateService automatically adds ".html".
            // ========================================================

            logger.LogInformation("Rendering Welcome.html for {Email}.", message.EmailAddress);

            var welcomeHtml = await emailTemplateService.RenderAsync
            (
                "Welcome",
                placeholders,
                stoppingToken
            );

            var welcomeEmail = new EmailMessage
                {
                    To = message.EmailAddress,
                    Subject = "Welcome to BookMyHall",
                    HtmlBody = welcomeHtml,
                    InlineAttachments = []
                };

            logger.LogInformation("Sending Welcome email to {Email}.", message.EmailAddress);
            await emailSender.SendAsync(welcomeEmail, stoppingToken);

            logger.LogInformation("Welcome email sent successfully to {Email}.", message.EmailAddress);

            // ========================================================
            // 9. Send Email Verification Email
            // ========================================================
            //
            // Template:
            //
            // Email/Templates/EmailVerified.html
            // ========================================================

            logger.LogInformation("Rendering EmailVerified.html for {Email}.", message.EmailAddress);

            var verificationHtml = await emailTemplateService.RenderAsync("EmailVerified", placeholders, stoppingToken);

            var verificationEmail = new EmailMessage
            {
                To = message.EmailAddress,

                Subject =
                    "Verify your BookMyHall account",

                HtmlBody =
                    verificationHtml,

                InlineAttachments =
                    []
            };

            logger.LogInformation("Sending verification email to {Email}.", message.EmailAddress);
            await emailSender.SendAsync(verificationEmail, stoppingToken);

            logger.LogInformation("Verification email sent successfully to {Email}.", message.EmailAddress);

            // ========================================================
            // 10. ACK RabbitMQ message
            // ========================================================
            //
            // ACK ONLY after BOTH emails are successfully sent.
            // ========================================================

            await _channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);

            logger.LogInformation("================================================");
            logger.LogInformation("User registration processing completed.");
            logger.LogInformation("UserId : {UserId}", message.UserId);
            logger.LogInformation("Email  : {Email}", message.EmailAddress);
            logger.LogInformation("Welcome email       : SUCCESS");
            logger.LogInformation("Verification email   : SUCCESS");
            logger.LogInformation("RabbitMQ message    : ACK");
            logger.LogInformation("================================================");
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("User registration email processing cancelled.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to process registration email message.");

            // ========================================================
            // NACK + REQUEUE
            // ========================================================

            if (_channel is not null && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _channel.BasicNackAsync
                    (
                        deliveryTag: eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: true,
                        cancellationToken: stoppingToken
                    );

                    logger.LogWarning
                    (
                        "RabbitMQ registration message requeued " +
                        "for retry."
                    );
                }
                catch (Exception nackException)
                {
                    logger.LogError(nackException, "Failed to NACK RabbitMQ message.");
                }
            }
        }
    }

    // ================================================================
    // Graceful shutdown
    // ================================================================

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping UserRegistrationConsumer.");

        // ============================================================
        // Close channel
        // ============================================================

        if (_channel is not null)
        {
            try
            {
                await _channel.CloseAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Error while closing RabbitMQ channel.");
            }

            _channel = null;
        }

        // ============================================================
        // Close connection
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

        await base.StopAsync(
            cancellationToken);
    }
}