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
        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<VerifyEmailResponse>.FailureResponse(
                string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest);
        }

        var user = await userRepository.GetByEmailAddressAsync(
            request.Email,
            cancellationToken);

        if (user is null)
        {
            return ApiResponse<VerifyEmailResponse>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.User),
                HttpStatusCode.NotFound);
        }

        var tokenHash = tokenHasher.Hash(request.Token);

        var verificationToken =
            await emailVerificationTokenRepository.GetActiveTokenAsync(
                user.UserId,
                tokenHash,
                cancellationToken);

        if (verificationToken is null)
        {
            return ApiResponse<VerifyEmailResponse>.FailureResponse(
                "The verification link is invalid or has expired.",
                HttpStatusCode.BadRequest);
        }

        user.VerifyEmail();

        verificationToken.MarkAsVerified();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await mediator.Publish(
            new EmailVerifiedEvent(
                user.UserId,
                user.FullName,
                user.EmailAddress),
            cancellationToken);

        return ApiResponse<VerifyEmailResponse>.SuccessResponse(
            new VerifyEmailResponse
            {
                Message = "Your email address has been verified successfully."
            },
            "Email verified successfully.");
    }
}