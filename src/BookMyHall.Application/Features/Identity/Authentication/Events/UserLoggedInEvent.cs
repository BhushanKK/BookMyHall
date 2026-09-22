namespace BookMyHall.Application.Events.Identity;

public sealed record UserLoggedInEvent(
    Guid UserId,
    Guid SessionId,
    DateTimeOffset LoginDate,
    string LoginMethod,
    string? IpAddress,
    string? UserAgent,
    string? Browser,
    string? OperatingSystem,
    string? DeviceType,
    string? LoginSource,
    bool IsMfaUsed);