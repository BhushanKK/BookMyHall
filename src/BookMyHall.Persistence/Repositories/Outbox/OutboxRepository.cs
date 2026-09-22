using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Outbox;
using BookMyHall.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories;

public sealed class OutboxRepository(BookMyHallDbContext context) : IOutboxRepository
{
    public async Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        return await context.Set<OutboxMessage>()
            .Where(x => x.ProcessedOn == null)
            .OrderBy(x => x.OccurredOn)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public Task MarkAsProcessedAsync(
        OutboxMessage message,
        CancellationToken cancellationToken = default)
    {
        message.ProcessedOn = DateTimeOffset.UtcNow;
        message.Error = null;
        message.UpdatedDate = DateTimeOffset.UtcNow;
        context.Set<OutboxMessage>().Update(message);
        return Task.CompletedTask;
    }

    public Task MarkAsFailedAsync(
        OutboxMessage message,
        string error,
        CancellationToken cancellationToken = default)
    {
        message.RetryCount++;
        message.Error = error;
        message.UpdatedDate = DateTimeOffset.UtcNow;
        context.Set<OutboxMessage>().Update(message);
        return Task.CompletedTask;
    }
}