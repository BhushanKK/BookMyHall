using System.Net;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Features.Authentication.Events;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Features.Identity.Authentication;

namespace BookMyHall.Application.Features.Authentication.Commands.VerifyEmail;

public sealed class VerifyEmailCommandHandler(
    IUserRepository userRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    ITokenHasher tokenHasher,
    IUnitOfWork unitOfWork,
    IValidator<VerifyEmailCommand> validator,
    IMessageHelper messageHelper,
    IMediator mediator)
    : IRequestHandler<VerifyEmailCommand, ApiResponse<VerifyEmailResponse>>
{
    public async Task<ApiResponse<VerifyEmailResponse>> Handle(
        VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        // ------------------------------------------------------------
        // 1. Validate request
        // ------------------------------------------------------------

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VerifyEmailResponse>.FailureResponse
            (
                message,
                HttpStatusCode.BadRequest
            );
        }

        // ------------------------------------------------------------
        // 2. Get user by UserId
        // ------------------------------------------------------------

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return ApiResponse<VerifyEmailResponse>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.NotFound
            );
        }

        // ------------------------------------------------------------
        // 3. Check if email is already verified
        // ------------------------------------------------------------

        if (user.IsEmailVerified)
        {
            return ApiResponse<VerifyEmailResponse>.FailureResponse
            (
                "Email address has already been verified.",
                HttpStatusCode.BadRequest
            );
        }

        // ------------------------------------------------------------
        // 4. Hash verification token
        // ------------------------------------------------------------

        var tokenHash = tokenHasher.Hash(request.Token);

        // ------------------------------------------------------------
        // 5. Find active verification token
        // ------------------------------------------------------------

        var verificationToken = await emailVerificationTokenRepository.GetActiveTokenAsync
        (
            user.UserId,
            tokenHash,
            cancellationToken
        );

        if (verificationToken is null)
        {
            return ApiResponse<VerifyEmailResponse>.FailureResponse
            (
                "The verification link is invalid or has expired.",
                HttpStatusCode.BadRequest
            );
        }

        // ------------------------------------------------------------
        // 6. Verify email
        // ------------------------------------------------------------

        user.VerifyEmail();

        // ------------------------------------------------------------
        // 7. Mark verification token as verified
        // ------------------------------------------------------------

        verificationToken.MarkAsVerified();

        // ------------------------------------------------------------
        // 8. Save changes
        // ------------------------------------------------------------

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // ------------------------------------------------------------
        // 9. Publish Email Verified event
        // ------------------------------------------------------------

        await mediator.Publish
        (
            new EmailVerifiedEvent(user.UserId, user.FullName, user.EmailAddress),
            cancellationToken
        );

        // ------------------------------------------------------------
        // 10. Determine whether password setup is required
        // ------------------------------------------------------------

        var passwordSetupRequired = string.IsNullOrWhiteSpace(user.PasswordHash);

        // ------------------------------------------------------------
        // 11. Build response
        // ------------------------------------------------------------

        var response = new VerifyEmailResponse
        {
            Message = "Your email address has been verified successfully.",
            UserId = user.UserId,
            IsEmailVerified =user.IsEmailVerified,
            PasswordSetupRequired = passwordSetupRequired
        };

        // ------------------------------------------------------------
        // 12. Return response
        // ------------------------------------------------------------

        return ApiResponse<VerifyEmailResponse>.SuccessResponse
        (
            response, 
            "Email verified successfully.",
            HttpStatusCode.OK
        );
    }
}