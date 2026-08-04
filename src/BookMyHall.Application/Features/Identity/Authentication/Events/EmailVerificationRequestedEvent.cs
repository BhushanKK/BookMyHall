using MediatR;

namespace BookMyHall.Application.Features.Authentication.Events;
public sealed record EmailVerificationRequestedEvent(
    Guid UserId,
    string UserName,
    string Email,
    string VerificationToken)
    : INotification;