using System.Net;
using FluentValidation;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Messaging;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Features.Identity.Authentication;

namespace BookMyHall.Application.Features.Authentication.Commands.VerifyEmail;

public sealed class VerifyEmailCommandHandler(
    IUserRepository userRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    ITokenHasher tokenHasher, IUnitOfWork unitOfWork,
    IValidator<VerifyEmailCommand> validator, IMessageHelper messageHelper,
    IMessagePublisher messagePublisher)
    : IRequestHandler<VerifyEmailCommand, ApiResponse<VerifyEmailResponse>>
{
    public async Task<ApiResponse<VerifyEmailResponse>> Handle(
        VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VerifyEmailResponse>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return ApiResponse<VerifyEmailResponse>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.NotFound
            );
        }

        if (user.IsEmailVerified)
            return ApiResponse<VerifyEmailResponse>.FailureResponse("Email address has already been verified.", HttpStatusCode.BadRequest);

        var tokenHash = tokenHasher.Hash(request.Token);

        var verificationToken = await emailVerificationTokenRepository.GetActiveTokenAsync(user.UserId, tokenHash, cancellationToken);

        if (verificationToken is null)
            return ApiResponse<VerifyEmailResponse>.FailureResponse("The verification link is invalid or has expired.", HttpStatusCode.BadRequest);

        user.VerifyEmail();

        verificationToken.MarkAsVerified();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var emailVerifiedMessage = new EmailVerifiedMessage(user.UserId, user.FullName, user.EmailAddress!);

        await messagePublisher.PublishAsync(emailVerifiedMessage,cancellationToken);

        var passwordSetupRequired = string.IsNullOrWhiteSpace(user.PasswordHash);

        var response = new VerifyEmailResponse
        {
            Message = "Your email address has been verified successfully.",
            UserId = user.UserId,
            IsEmailVerified = user.IsEmailVerified,
            PasswordSetupRequired = passwordSetupRequired
        };

        return ApiResponse<VerifyEmailResponse>.SuccessResponse(response, "Email verified successfully.", HttpStatusCode.OK);
    }
}