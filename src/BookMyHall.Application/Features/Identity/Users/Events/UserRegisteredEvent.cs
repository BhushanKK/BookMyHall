using MediatR;

namespace BookMyHall.Application.Features.Authentication.Events;

public sealed record UserRegisteredEvent
(Guid UserId, string UserName, string Email)
    : INotification;