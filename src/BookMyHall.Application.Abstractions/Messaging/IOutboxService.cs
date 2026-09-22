namespace BookMyHall.Application.Abstractions.Messaging;

public interface IOutboxService
{
    Task AddAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default);
}