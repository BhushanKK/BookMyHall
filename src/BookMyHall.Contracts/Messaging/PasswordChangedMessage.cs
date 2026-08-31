namespace BookMyHall.Contracts.Messaging;

public sealed record PasswordChangedMessage(
    Guid UserId,
    string FullName,
    string EmailAddress
);