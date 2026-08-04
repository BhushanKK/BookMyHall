using MediatR;

namespace BookMyHall.Application.Features.Authentication.Events;

public sealed record PasswordResetCompletedEvent(
    Guid UserId,
    string UserName,
    string Email) : INotification;