namespace BookMyHall.Application.Abstractions.Audit;
public interface IAuditRequestContext
{
    Guid CorrelationId { get; }

    string? IpAddress { get; }

    string? UserAgent { get; }
}