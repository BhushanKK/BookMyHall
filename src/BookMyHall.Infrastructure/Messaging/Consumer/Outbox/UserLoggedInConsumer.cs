using System.Text;
using System.Text.Json;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Events.Identity;
using BookMyHall.Domain.Audit;
using BookMyHall.Domain.Common;
using BookMyHall.Infrastructure.Authentication;
using BookMyHall.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BookMyHall.Infrastructure.Messaging.Consumers;

public sealed class UserLoggedInConsumer(
    IOptions<RabbitMqOptions> rabbitMqOptions,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<UserLoggedInConsumer> logger)
    : BackgroundService
{
    private static readonly string QueueName = RabbitMqKeys.UserLoggedInQueueName;
    private static readonly string RoutingKey = RabbitMqKeys.UserLoggedInRoutingKey;
    private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqOptions.Value;
    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation(
                "Starting UserLoggedInConsumer. Host: {Host}, Port: {Port}, Exchange: {Exchange}, Queue: {Queue}, RoutingKey: {RoutingKey}",
                _rabbitMqOptions.HostName,
                _rabbitMqOptions.Port,
                _rabbitMqOptions.ExchangeName,
                QueueName,
                RoutingKey);

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.HostName,
                Port = _rabbitMqOptions.Port,
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password,
                VirtualHost = _rabbitMqOptions.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);

            logger.LogInformation(
                "RabbitMQ connection created successfully. IsOpen: {IsOpen}",
                _connection.IsOpen);

            _channel = await _connection.CreateChannelAsync(
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ channel created successfully. IsOpen: {IsOpen}",
                _channel.IsOpen);

            await _channel.ExchangeDeclareAsync(
                exchange: _rabbitMqOptions.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(
                queue: QueueName,
                exchange: _rabbitMqOptions.ExchangeName,
                routingKey: RoutingKey,
                cancellationToken: stoppingToken);

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ QoS configured. PrefetchCount: 1");

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
                "UserLoggedInConsumer started successfully.");

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "UserLoggedInConsumer cancellation requested.");
        }
        catch (Exception exception)
        {
            logger.LogCritical(
                exception,
                "UserLoggedInConsumer stopped unexpectedly.");

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
            "RabbitMQ UserLoggedInEvent received. DeliveryTag: {DeliveryTag}, Queue: {Queue}",
            eventArgs.DeliveryTag,
            QueueName);

        try
        {
            var json = Encoding.UTF8.GetString(
                eventArgs.Body.ToArray());

            logger.LogDebug(
                "RabbitMQ UserLoggedInEvent payload: {Message}",
                json);

            var message =
                JsonSerializer.Deserialize<UserLoggedInEvent>(json);

            if (message is null)
            {
                logger.LogWarning(
                    "Received invalid UserLoggedInEvent.");

                await _channel.BasicNackAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);

                return;
            }

            logger.LogInformation(
                "Processing UserLoggedInEvent. UserId: {UserId}, SessionId: {SessionId}",
                message.UserId,
                message.SessionId);

            using var scope =
                serviceScopeFactory.CreateScope();

            var userLoginHistoryRepository =
                scope.ServiceProvider
                    .GetRequiredService<IUserLoginHistoryRepository>();

            var unitOfWork =
                scope.ServiceProvider
                    .GetRequiredService<IUnitOfWork>();

            var loginHistory = new UserLoginHistory
            {
                UserLoginHistoryId = Guid.NewGuid(),
                UserId = message.UserId,
                SessionId = message.SessionId,
                LoginDate = message.LoginDate,
                LoginStatus = LoginStatuses.Success,
                LoginMethod = message.LoginMethod,
                IpAddress = message.IpAddress ?? "Unknown",
                UserAgent = message.UserAgent ?? "Unknown",
                Browser = message.Browser ?? "Unknown",
                OperatingSystem = message.OperatingSystem ?? "Unknown",
                DeviceType = message.DeviceType ?? "Unknown",
                LoginSource = message.LoginSource ?? "Unknown",
                IsMfaUsed = message.IsMfaUsed
            };

            await userLoginHistoryRepository.AddAsync(
                loginHistory,
                stoppingToken);

            await unitOfWork.SaveChangesAsync(
                stoppingToken);

            await _channel.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "UserLoginHistory created and RabbitMQ message ACKed successfully. UserId: {UserId}, SessionId: {SessionId}",
                message.UserId,
                message.SessionId);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "User login history processing cancelled.");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Failed to process UserLoggedInEvent. DeliveryTag: {DeliveryTag}",
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
                        "RabbitMQ UserLoggedInEvent rejected. DeliveryTag: {DeliveryTag}",
                        eventArgs.DeliveryTag);
                }
                catch (Exception nackException)
                {
                    logger.LogError(
                        nackException,
                        "Failed to NACK RabbitMQ UserLoggedInEvent. DeliveryTag: {DeliveryTag}",
                        eventArgs.DeliveryTag);
                }
            }
        }
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Stopping UserLoggedInConsumer.");

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