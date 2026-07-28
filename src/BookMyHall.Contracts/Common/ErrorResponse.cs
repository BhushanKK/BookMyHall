using BookMyHall.Contracts.Constants;

namespace BookMyHall.Contracts.Common;

public sealed class ErrorResponse
{
    public ErrorType Type { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? Field { get; init; }
}