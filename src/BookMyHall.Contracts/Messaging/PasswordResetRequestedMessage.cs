namespace BookMyHall.Contracts.Messaging;

public sealed record PasswordResetRequestedMessage(
    Guid UserId,
    string FullName,
    string EmailAddress,
    string ResetToken);