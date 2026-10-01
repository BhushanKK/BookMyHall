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

public sealed class PasswordChangedConsumer(
IOptions<RabbitMqOptions> rabbitMqOptions,
IOptions<FrontendOptions> frontendOptions,
IServiceScopeFactory serviceScopeFactory,
ILogger<PasswordChangedConsumer> logger)
: BackgroundService
{
    private const string QueueName = RabbitMqKeys.PasswordChangedQueueName;
    private const string RoutingKey = RabbitMqKeys.PasswordChangedRoutingKey;
    private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqOptions.Value;
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;
    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Starting PasswordChangedConsumer.");
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
            logger.LogInformation("RabbitMQ connection created successfully.");

            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
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
                await ProcessMessageAsync(eventArgs, stoppingToken);
            };

            await _channel.BasicConsumeAsync
            (
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            logger.LogInformation("PasswordChangedConsumer started successfully. Queue: {QueueName}, RoutingKey: {RoutingKey}", QueueName, RoutingKey);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("PasswordChangedConsumer cancellation requested.");
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "PasswordChangedConsumer stopped unexpectedly.");
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

            logger.LogDebug("RabbitMQ password changed message received: {Message}", json);

            var message = JsonSerializer.Deserialize<PasswordChangedMessage>(json);

            if (message is null)
            {
                logger.LogWarning("Received invalid PasswordChangedMessage.");

                await _channel.BasicNackAsync
                (
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken
                );

                return;
            }

            logger.LogInformation("Processing password changed message. UserId: {UserId}, Email: {Email}", message.UserId, message.EmailAddress);

            using var scope = serviceScopeFactory.CreateScope();

            var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
            var baseUrl = _frontendOptions.BaseUrl.TrimEnd('/');

            var placeholders = new Dictionary<string, string>
            {
                ["UserName"] = message.FullName,
                ["WebsiteUrl"] = baseUrl,
                ["CurrentYear"] = DateTime.UtcNow.Year.ToString()
            };

            logger.LogInformation("Rendering PasswordChanged template for {Email}.", message.EmailAddress);

            var passwordChangedHtml = await emailTemplateService.RenderAsync(EmailTemplateConstants.PasswordChanged, placeholders, stoppingToken);

            var passwordChangedEmail = new EmailMessage
            {
                To = message.EmailAddress,
                Subject = "Your BookMyLawns password was changed",
                HtmlBody = passwordChangedHtml
            };

            logger.LogInformation("Sending password changed email to {Email}.", message.EmailAddress);

            await emailSender.SendAsync(passwordChangedEmail, stoppingToken);

            logger.LogInformation("Password changed email sent successfully to {Email}.", message.EmailAddress);

            await _channel.BasicAckAsync
            (
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken
            );

            logger.LogInformation("Password changed processing completed successfully. UserId: {UserId}, Email: {Email}", message.UserId, message.EmailAddress);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Password changed email processing cancelled.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to process password changed email message.");

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

                    logger.LogWarning("RabbitMQ password changed message rejected after processing failure.");
                }
                catch (Exception nackException)
                {
                    logger.LogError(nackException, "Failed to NACK RabbitMQ password changed message.");
                }
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping PasswordChangedConsumer.");

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
