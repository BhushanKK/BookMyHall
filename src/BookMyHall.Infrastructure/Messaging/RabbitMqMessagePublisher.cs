using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Contracts.Messaging;
using BookMyHall.Infrastructure.Configuration;

namespace BookMyHall.Infrastructure.Messaging;

public sealed class RabbitMqMessagePublisher(
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqMessagePublisher> logger)
    : IMessagePublisher
{
    private readonly RabbitMqOptions _options = options.Value;

    public async Task PublishAsync<T>(
        T message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        logger.LogInformation(
            "RabbitMQ publish started. MessageType: {MessageType}",
            typeof(T).Name);

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        logger.LogInformation(
            "Creating RabbitMQ connection to {Host}:{Port}",
            _options.HostName,
            _options.Port);

        await using var connection =
            await factory.CreateConnectionAsync(
                cancellationToken);

        logger.LogInformation(
            "RabbitMQ publisher connection created.");

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

        logger.LogInformation(
            "RabbitMQ publisher channel created.");

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "RabbitMQ exchange declared: {ExchangeName}",
            _options.ExchangeName);

        var routingKey = GetRoutingKey<T>();

        var body =
            JsonSerializer.SerializeToUtf8Bytes(message);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        logger.LogInformation(
            "Publishing RabbitMQ message. Exchange: {Exchange}, RoutingKey: {RoutingKey}",
            _options.ExchangeName,
            routingKey);

        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "RabbitMQ message published successfully. MessageType: {MessageType}, RoutingKey: {RoutingKey}",
            typeof(T).Name,
            routingKey);
    }

    private static string GetRoutingKey<T>()
    {
        return typeof(T) switch
        {
            var type when type == typeof(UserRegisteredMessage)
                => RabbitMqKeys.UserRegistrationRoutingKey,

            var type when type == typeof(PasswordChangedMessage)
                => RabbitMqKeys.PasswordChangedRoutingKey,

            var type when type == typeof(PasswordResetRequestedMessage)
                => RabbitMqKeys.PasswordResetRoutingKey,

            var type when type == typeof(PasswordResetSuccessMessage)
                => RabbitMqKeys.PasswordResetSuccessRoutingKey,

            var type when type == typeof(EmailVerifiedMessage)
                => RabbitMqKeys.EmailVerifiedRoutingKey,

            var type when type == typeof(EmailVerificationRequestedMessage)
                => RabbitMqKeys.EmailVerificationRoutingKey,

             var type when type == typeof(HallImageUploadedMessage)
                => RabbitMqKeys.HallImageUploadedRoutingKey,

            _ => throw new InvalidOperationException(
                $"No RabbitMQ routing key configured for message type '{typeof(T).Name}'.")
        };
    }
}