namespace BookMyHall.Application.Abstractions.Security;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    string? FullName { get; }
    string? MobileNumber { get; }
    string? EmailAddress { get; }
    IReadOnlyList<string> Roles { get; }
}