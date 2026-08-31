using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Contracts.Messaging;
using BookMyHall.Infrastructure.Configuration;

namespace BookMyHall.Infrastructure.Messaging;

public sealed class RabbitMqMessagePublisher(IOptions<RabbitMqOptions> options)
    : IMessagePublisher
{
    private readonly RabbitMqOptions _options = options.Value;

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync
        (
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        var routingKey = GetRoutingKey<T>();
        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync
        (
            exchange: _options.ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken
        );
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

            _ => throw new InvalidOperationException
            (
                $"No RabbitMQ routing key configured for message type '{typeof(T).Name}'."
            )
        };
    }
}