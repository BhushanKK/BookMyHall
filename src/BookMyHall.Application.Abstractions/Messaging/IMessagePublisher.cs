namespace BookMyHall.Application.Abstractions.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message,CancellationToken cancellationToken = default);
}