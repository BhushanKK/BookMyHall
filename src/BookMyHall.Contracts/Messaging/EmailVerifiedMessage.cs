namespace BookMyHall.Contracts.Messaging;

public sealed record EmailVerifiedMessage(
    Guid UserId,
    string FullName,
    string EmailAddress);