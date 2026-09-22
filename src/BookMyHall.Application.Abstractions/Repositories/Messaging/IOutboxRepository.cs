using BookMyHall.Domain.Outbox;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IOutboxRepository
{
    Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(
        int batchSize,
        CancellationToken cancellationToken = default);

    Task MarkAsProcessedAsync(
        OutboxMessage message,
        CancellationToken cancellationToken = default);

    Task MarkAsFailedAsync(
        OutboxMessage message,
        string error,
        CancellationToken cancellationToken = default);
}