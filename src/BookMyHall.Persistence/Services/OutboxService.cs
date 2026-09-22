using System.Text.Json;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Domain.Outbox;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Services;

public sealed class OutboxService(BookMyHallDbContext context) : IOutboxService
{
    public async Task AddAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var message = new OutboxMessage
        {
            OutboxMessageId = Guid.NewGuid(),
            EventType = typeof(TEvent).Name,
            Payload = JsonSerializer.Serialize(@event),
            OccurredOn = DateTimeOffset.UtcNow
        };

        await context.Set<OutboxMessage>()
            .AddAsync(message, cancellationToken);
    }
}