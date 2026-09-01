namespace BookMyHall.Contracts.Messaging;

public sealed record EmailVerificationRequestedMessage(
    Guid UserId,
    string FullName,
    string EmailAddress,
    string VerificationToken,
    int ExpiryMinutes
);