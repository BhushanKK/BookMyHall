namespace BookMyHall.Contracts.Common;

public sealed record ApiErrorResponse
(
    int StatusCode,
    string Message,
    string TraceId,
    DateTimeOffset Timestamp
);