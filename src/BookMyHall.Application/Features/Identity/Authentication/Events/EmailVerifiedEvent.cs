using MediatR;

namespace BookMyHall.Application.Features.Authentication.Events;

public sealed record EmailVerifiedEvent(
    Guid UserId,
    string UserName,
    string Email)
    : INotification;