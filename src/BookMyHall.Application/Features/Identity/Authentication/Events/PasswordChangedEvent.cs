using MediatR;

namespace BookMyHall.Application.Features.Authentication.Events;

public sealed record PasswordChangedEvent(
    Guid UserId,
    string UserName,
    string Email) : INotification;