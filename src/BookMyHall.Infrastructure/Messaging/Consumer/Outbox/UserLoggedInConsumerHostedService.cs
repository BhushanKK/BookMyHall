using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using BookMyHall.Infrastructure.Configuration;

namespace BookMyHall.Infrastructure.Messaging.Consumers;

public sealed class UserLoggedInConsumerHostedService(
    IOptions<RabbitMqOptions> options,
    UserLoggedInConsumer consumer,
    ILogger<UserLoggedInConsumerHostedService> logger)
    : BackgroundService
{
    private readonly RabbitMqOptions _options = options.Value;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        await using var connection =
            await factory.CreateConnectionAsync(stoppingToken);

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: RabbitMqKeys.UserLoggedInQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: RabbitMqKeys.UserLoggedInQueueName,
            exchange: _options.ExchangeName,
            routingKey: RabbitMqKeys.UserLoggedInRoutingKey,
            cancellationToken: stoppingToken);

        var rabbitConsumer = new AsyncEventingBasicConsumer(channel);

        rabbitConsumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                await consumer.ConsumeAsync(
                    args.Body,
                    stoppingToken);

                await channel.BasicAckAsync(
                    args.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);

                logger.LogInformation(
                    "UserLoggedInEvent consumed successfully. DeliveryTag: {DeliveryTag}",
                    args.DeliveryTag);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to consume UserLoggedInEvent. DeliveryTag: {DeliveryTag}",
                    args.DeliveryTag);

                await channel.BasicNackAsync(
                    args.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: RabbitMqKeys.UserLoggedInQueueName,
            autoAck: false,
            consumer: rabbitConsumer,
            cancellationToken: stoppingToken);

        logger.LogInformation(
            "UserLoggedInConsumer started. Queue: {Queue}",
            RabbitMqKeys.UserLoggedInQueueName);

        await Task.Delay(
            Timeout.InfiniteTimeSpan,
            stoppingToken);
    }
}