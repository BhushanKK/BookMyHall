using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Outbox;

public sealed class OutboxMessage : BaseEntity
{
    public Guid OutboxMessageId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTimeOffset OccurredOn { get; set; }
    public DateTimeOffset? ProcessedOn { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; }
}