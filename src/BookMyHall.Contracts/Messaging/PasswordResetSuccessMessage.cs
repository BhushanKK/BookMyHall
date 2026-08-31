namespace BookMyHall.Contracts.Messaging;

public sealed record PasswordResetSuccessMessage(
    Guid UserId,
    string FullName,
    string EmailAddress);