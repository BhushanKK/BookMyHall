namespace BookMyHall.Contracts.Messaging;

public sealed record UserRegisteredMessage(
    Guid UserId,
    string FullName,
    string EmailAddress,
    string VerificationToken,
    int ExpiryMinutes);