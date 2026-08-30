using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BookMyHall.Infrastructure.Messaging;

public sealed class RabbitMqTopology(IOptions<RabbitMqOptions> options)
{
    private readonly RabbitMqOptions _options = options.Value;

    public async Task ConfigureAsync(CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);

        await using var channel =await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        // Exchange
        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        // Queue
        await channel.QueueDeclareAsync(
            queue: "identity.user.registered",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        // Binding
        await channel.QueueBindAsync(
            queue: "identity.user.registered",
            exchange: _options.ExchangeName,
            routingKey: "identity.user.registered",
            cancellationToken: cancellationToken);
    }
}