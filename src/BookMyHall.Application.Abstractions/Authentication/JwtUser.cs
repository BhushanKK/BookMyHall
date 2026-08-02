namespace BookMyHall.Application.Abstractions.Authentication;

public sealed class JwtUser
{
    public Guid UserId { get; init; }
    public string MobileNumber { get; init; } = string.Empty;
    public string? EmailAddress { get; init; }
    public string FullName { get; init; } = string.Empty;
    public int TokenVersion { get; init; }
    public IReadOnlyCollection<string> Roles { get; init; } = [];
}