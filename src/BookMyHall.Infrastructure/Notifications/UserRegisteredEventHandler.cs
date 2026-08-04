using MediatR;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Features.Authentication.Events;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Infrastructure.Notifications;

public sealed class UserRegisteredEventHandler(
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    ITokenGenerator tokenGenerator,
    ITokenHasher tokenHasher,
    IUnitOfWork unitOfWork,
    IMediator mediator)
    : INotificationHandler<UserRegisteredEvent>
{
    private const int EmailVerificationExpiryInMinutes = 30;

    public async Task Handle(
        UserRegisteredEvent notification,
        CancellationToken cancellationToken)
    {
        await emailVerificationTokenRepository.DeleteByUserIdAsync(
            notification.UserId,
            cancellationToken);

        var verificationToken =
            tokenGenerator.GenerateEmailVerificationToken();

        var tokenHash =
            tokenHasher.Hash(verificationToken);

        var entity = EmailVerificationToken.Create(
            notification.UserId,
            tokenHash,
            DateTimeOffset.UtcNow.AddMinutes(
                EmailVerificationExpiryInMinutes));

        await emailVerificationTokenRepository.AddAsync(
            entity,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await mediator.Publish(
            new EmailVerificationRequestedEvent(
                notification.UserId,
                notification.UserName,
                notification.Email,
                verificationToken),
            cancellationToken);
    }
}