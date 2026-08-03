using MediatR;

namespace BookMyHall.Application.Features.Authentication.Events;

public sealed record PasswordResetRequestedEvent(
    Guid UserId,
    string UserName,
    string Email,
    string ResetToken)
    : INotification;