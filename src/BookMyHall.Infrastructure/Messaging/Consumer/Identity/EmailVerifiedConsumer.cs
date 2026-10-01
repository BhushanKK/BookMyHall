using System.Text;
using System.Text.Json;
using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Contracts.Messaging;
using BookMyHall.Infrastructure.Configuration;
using BookMyHall.Infrastructure.Options;
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
IServiceScopeFactory serviceScopeFactory,
ILogger<EmailVerifiedConsumer> logger)
: BackgroundService
{
    private const string QueueName = "identity.user.email-verified";
    private const string RoutingKey = "identity.user.email-verified";
    private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqOptions.Value;
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;
    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Starting EmailVerifiedConsumer.");
            logger.LogInformation("Configured Frontend BaseUrl: {BaseUrl}", _frontendOptions.BaseUrl);

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

            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
            logger.LogInformation("RabbitMQ channel created successfully for EmailVerifiedConsumer.");

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
                await ProcessMessageAsync(eventArgs, stoppingToken);
            };

            await _channel.BasicConsumeAsync
            (
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken
                );

            logger.LogInformation("EmailVerifiedConsumer started successfully. Queue: {QueueName}, RoutingKey: {RoutingKey}", QueueName, RoutingKey);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("EmailVerifiedConsumer cancellation requested.");
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "EmailVerifiedConsumer stopped unexpectedly.");
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

            logger.LogDebug("RabbitMQ EmailVerifiedMessage received: {Message}", json);

            var message = JsonSerializer.Deserialize<EmailVerifiedMessage>(json);

            if (message is null)
            {
                logger.LogWarning("Received invalid EmailVerifiedMessage.");

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
                "Processing email verified event. UserId: {UserId}, Email: {Email}",
                message.UserId,
                message.EmailAddress
            );

            using var scope = serviceScopeFactory.CreateScope();

            var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var baseUrl = _frontendOptions.BaseUrl.TrimEnd('/');

            var passwordSetupUrl =
                $"{baseUrl}/set-password" +
                $"?userId={Uri.EscapeDataString(message.UserId.ToString())}";

            var placeholders = new Dictionary<string, string>
            {
                ["UserName"] = message.FullName,
                ["PasswordSetupLink"] = passwordSetupUrl,
                ["WebsiteUrl"] = baseUrl,
                ["CurrentYear"] = DateTime.UtcNow.Year.ToString()
            };

            logger.LogInformation("Rendering EmailVerified template for {Email}.", message.EmailAddress);

            var emailHtml = await emailTemplateService.RenderAsync
            (
                EmailTemplateConstants.EmailVerified,
                placeholders,
                stoppingToken
            );

            var email = new EmailMessage
            {
                To = message.EmailAddress,
                Subject = "Email verified successfully - BookMyLawns",
                HtmlBody = emailHtml
            };

            logger.LogInformation("Sending email verified notification to {Email}.", message.EmailAddress);

            await emailSender.SendAsync(email, stoppingToken);

            logger.LogInformation("Email verified notification sent successfully to {Email}.", message.EmailAddress);

            await _channel.BasicAckAsync
            (
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken
            );

            logger.LogInformation
            (
                "EmailVerifiedMessage processed successfully. UserId: {UserId}",
                message.UserId
            );
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Email verified email processing cancelled.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to process EmailVerifiedMessage.");

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

                    logger.LogWarning("RabbitMQ EmailVerifiedMessage rejected after processing failure.");
                }
                catch (Exception nackException)
                {
                    logger.LogError(nackException, "Failed to NACK EmailVerifiedMessage.");
                }
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping EmailVerifiedConsumer.");

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

        if (_connection is not null)
        {
            try
            {
                await _connection.CloseAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Error while closing RabbitMQ connection.");
            }

            _connection = null;
        }

        await base.StopAsync(cancellationToken);
    }
}
