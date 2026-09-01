using BookMyHall.Infrastructure.Configuration;
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

        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync
        (
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync
        (
            queue: RabbitMqKeys.UserRegistrationQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync
        (
            queue: RabbitMqKeys.UserRegistrationQueueName,
            exchange: _options.ExchangeName,
            routingKey: RabbitMqKeys.UserRegistrationRoutingKey,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync
        (
            queue: RabbitMqKeys.PasswordChangedQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync
        (
            queue: RabbitMqKeys.PasswordChangedQueueName,
            exchange: _options.ExchangeName,
            routingKey: RabbitMqKeys.PasswordChangedRoutingKey,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync
        (
            queue: RabbitMqKeys.PasswordResetQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync
        (
            queue: RabbitMqKeys.PasswordResetQueueName,
            exchange: _options.ExchangeName,
            routingKey: RabbitMqKeys.PasswordResetRoutingKey,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync
        (
            queue: RabbitMqKeys.PasswordResetSuccessQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync
        (
            queue: RabbitMqKeys.PasswordResetSuccessQueueName,
            exchange: _options.ExchangeName,
            routingKey: RabbitMqKeys.PasswordResetSuccessRoutingKey,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync
        (
            queue: RabbitMqKeys.EmailVerifiedQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync
        (
            queue: RabbitMqKeys.EmailVerifiedQueueName,
            exchange: _options.ExchangeName,
            routingKey: RabbitMqKeys.EmailVerifiedRoutingKey,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync
        (
            queue: RabbitMqKeys.EmailVerificationQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync
        (
            queue: RabbitMqKeys.EmailVerificationQueueName,
            exchange: _options.ExchangeName,
            routingKey: RabbitMqKeys.EmailVerificationRoutingKey,
            cancellationToken: cancellationToken
        );
    }
}