using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Features.Authentication.Events;

namespace BookMyHall.Application.Features.Authentication.Commands.ResendVerificationEmail;

public sealed class ResendVerificationEmailCommandHandler(
    IUserRepository userRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    ITokenGenerator tokenGenerator,
    ITokenHasher tokenHasher,
    IUnitOfWork unitOfWork,
    IMediator mediator)
    : IRequestHandler<ResendVerificationEmailCommand, ApiResponse<ResendVerificationEmailResponse>>
{
    private const int EmailVerificationTokenExpiryInMinutes = 30;

    public async Task<ApiResponse<ResendVerificationEmailResponse>> Handle(
        ResendVerificationEmailCommand request,
        CancellationToken cancellationToken)
    {
        var response = CreateSuccessResponse();

        var user = await userRepository.GetByEmailAddressAsync(request.Email, cancellationToken);

        // Prevent email enumeration
        if (user is null)
            return response;

        // Already verified
        if (user.IsEmailVerified)
            return response;

        // Remove existing verification tokens
        await emailVerificationTokenRepository.DeleteByUserIdAsync(user.UserId, cancellationToken);

        // Generate new verification token
        var verificationToken = tokenGenerator.GenerateEmailVerificationToken();

        // Hash before storing
        var tokenHash = tokenHasher.Hash(verificationToken);

        var emailVerificationToken = EmailVerificationToken.Create
        (
            user.UserId,
            tokenHash,
            DateTimeOffset.UtcNow.AddMinutes(EmailVerificationTokenExpiryInMinutes)
        );

        await emailVerificationTokenRepository.AddAsync(emailVerificationToken, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Send verification email
        await mediator.Publish
        (
            new EmailVerificationRequestedEvent(user.UserId, user.FullName, user.EmailAddress, verificationToken),
            cancellationToken
        );

        return response;
    }

    private static ApiResponse<ResendVerificationEmailResponse> CreateSuccessResponse()
    {
        return ApiResponse<ResendVerificationEmailResponse>.SuccessResponse(
            new ResendVerificationEmailResponse
            {
                Message = "If an account exists and the email address is not verified, a verification email has been sent."
            });
    }
}