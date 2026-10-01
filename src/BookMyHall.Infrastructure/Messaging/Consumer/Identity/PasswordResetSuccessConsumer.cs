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

public sealed class PasswordResetSuccessConsumer(
IOptions<RabbitMqOptions> rabbitMqOptions,
IOptions<FrontendOptions> frontendOptions,
IServiceScopeFactory serviceScopeFactory,
ILogger<PasswordResetSuccessConsumer> logger)
: BackgroundService
{
    private const string QueueName = RabbitMqKeys.PasswordResetSuccessQueueName;
    private const string RoutingKey = RabbitMqKeys.PasswordResetSuccessRoutingKey;
    private const int PasswordResetExpiryMinutes = 30;
    private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqOptions.Value;
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;
    private IConnection? _connection;
    private IChannel? _channel;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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

            logger.LogInformation("PasswordResetSuccessConsumer started successfully. Queue: {QueueName}, RoutingKey: {RoutingKey}", QueueName, RoutingKey);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("PasswordResetSuccessConsumer cancellation requested.");
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "PasswordResetSuccessConsumer stopped unexpectedly.");
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

            logger.LogDebug("RabbitMQ password reset success message received: {Message}", json);

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

            logger.LogInformation("Processing password reset success message. UserId: {UserId}, Email: {Email}", message.UserId, message.EmailAddress);

            using var scope = serviceScopeFactory.CreateScope();

            var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            if (string.IsNullOrWhiteSpace(_frontendOptions.BaseUrl))
                throw new InvalidOperationException("Frontend:BaseUrl is not configured.");

            var baseUrl = _frontendOptions.BaseUrl.TrimEnd('/');

            var placeholders = new Dictionary<string, string>
            {
                ["UserName"] = message.FullName,
                ["WebsiteUrl"] = baseUrl,
                ["ExpiryMinutes"] = PasswordResetExpiryMinutes.ToString(),
                ["CurrentYear"] = DateTime.UtcNow.Year.ToString()
            };

            logger.LogInformation("Rendering PasswordResetSuccess template for {Email}.", message.EmailAddress);

            var passwordResetSuccessHtml = await emailTemplateService.RenderAsync(EmailTemplateConstants.PasswordResetSuccess, placeholders, stoppingToken);

            if (string.IsNullOrWhiteSpace(passwordResetSuccessHtml))
                throw new InvalidOperationException("PasswordResetSuccess email template rendered empty.");

            var passwordResetSuccessEmail = new EmailMessage
            {
                To = message.EmailAddress,
                Subject = "Your BookMyLawns password was changed successfully",
                HtmlBody = passwordResetSuccessHtml
            };

            logger.LogInformation("Sending password reset success email to {Email}.", message.EmailAddress);
            await emailSender.SendAsync(passwordResetSuccessEmail, stoppingToken);
            logger.LogInformation("Password reset success email sent successfully to {Email}.", message.EmailAddress);
            await _channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            logger.LogInformation("Password reset success message acknowledged. UserId: {UserId}", message.UserId);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Password reset success email processing cancelled.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to process password reset success email message.");

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

                    logger.LogWarning("RabbitMQ password reset success message rejected after processing failure.");
                }
                catch (Exception nackException)
                {
                    logger.LogError(nackException, "Failed to NACK password reset success message.");
                }
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
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
