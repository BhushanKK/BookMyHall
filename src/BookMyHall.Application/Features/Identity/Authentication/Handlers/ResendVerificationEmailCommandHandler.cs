using MediatR;

using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Messaging;

using BookMyHall.Domain.Entities.Identity;

using Microsoft.Extensions.Logging;

namespace BookMyHall.Application.Features.Authentication.Commands.ResendVerificationEmail;

public sealed class ResendVerificationEmailCommandHandler(
    IUserRepository userRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    ITokenGenerator tokenGenerator,
    ITokenHasher tokenHasher,
    IUnitOfWork unitOfWork,
    IMessagePublisher messagePublisher,
    ILogger<ResendVerificationEmailCommandHandler> logger)
    : IRequestHandler<
        ResendVerificationEmailCommand,
        ApiResponse<ResendVerificationEmailResponse>>
{
    private const int EmailVerificationTokenExpiryInMinutes = 30;

    public async Task<ApiResponse<ResendVerificationEmailResponse>> Handle(
        ResendVerificationEmailCommand request,
        CancellationToken cancellationToken)
    {
        // ------------------------------------------------------------
        // 1. Generic success response
        // ------------------------------------------------------------

        var response = CreateSuccessResponse();

        // ------------------------------------------------------------
        // 2. Find user
        // ------------------------------------------------------------

        var user = await userRepository.GetByEmailAddressAsync(
            request.Email,
            cancellationToken);

        // ------------------------------------------------------------
        // 3. Prevent email enumeration
        // ------------------------------------------------------------

        if (user is null)
        {
            return response;
        }

        // ------------------------------------------------------------
        // 4. Already verified
        // ------------------------------------------------------------

        if (user.IsEmailVerified)
        {
            return response;
        }

        // ------------------------------------------------------------
        // 5. Remove existing verification tokens
        // ------------------------------------------------------------

        await emailVerificationTokenRepository.DeleteByUserIdAsync(
            user.UserId,
            cancellationToken);

        // ------------------------------------------------------------
        // 6. Generate new verification token
        // ------------------------------------------------------------

        var verificationToken =
            tokenGenerator.GenerateEmailVerificationToken();

        // ------------------------------------------------------------
        // 7. Hash token before storing
        // ------------------------------------------------------------

        var tokenHash =
            tokenHasher.Hash(verificationToken);

        // ------------------------------------------------------------
        // 8. Create verification token entity
        // ------------------------------------------------------------

        var emailVerificationToken =
            EmailVerificationToken.Create(
                user.UserId,
                tokenHash,
                DateTimeOffset.UtcNow.AddMinutes(
                    EmailVerificationTokenExpiryInMinutes));

        // ------------------------------------------------------------
        // 9. Save token
        // ------------------------------------------------------------

        await emailVerificationTokenRepository.AddAsync(
            emailVerificationToken,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // ------------------------------------------------------------
        // 10. Create RabbitMQ message
        // ------------------------------------------------------------

        var message = new EmailVerificationRequestedMessage(
                user.UserId,
                user.FullName,
                user.EmailAddress!,
                verificationToken,
                EmailVerificationTokenExpiryInMinutes);

        // ------------------------------------------------------------
        // 11. Publish to RabbitMQ
        // ------------------------------------------------------------

        logger.LogInformation(
            "Publishing verification email message. UserId: {UserId}",
            user.UserId);

        await messagePublisher.PublishAsync(
            message,
            cancellationToken);

        logger.LogInformation(
            "Verification email message published successfully. UserId: {UserId}",
            user.UserId);

        // ------------------------------------------------------------
        // 12. Return generic response
        // ------------------------------------------------------------

        return response;
    }

    private static ApiResponse<ResendVerificationEmailResponse>
        CreateSuccessResponse()
    {
        return ApiResponse<ResendVerificationEmailResponse>.SuccessResponse(
            new ResendVerificationEmailResponse
            {
                Message =
                    "If an account exists and the email address is not verified, " +
                    "a verification email has been sent."
            });
    }
}