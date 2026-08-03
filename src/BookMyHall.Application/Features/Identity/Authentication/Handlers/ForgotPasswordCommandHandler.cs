using MediatR;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Features.Authentication.Commands.ForgotPassword;
using BookMyHall.Application.Features.Authentication.Events;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity.Authentication.Handlers;

public sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    ITokenGenerator tokenGenerator,
    ITokenHasher tokenHasher,
    IUnitOfWork unitOfWork,
    IMediator mediator)
    : IRequestHandler<ForgotPasswordCommand, ApiResponse<ForgotPasswordResponse>>
{
    private const int PasswordResetTokenExpiryInMinutes = 30;

    public async Task<ApiResponse<ForgotPasswordResponse>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var response = CreateSuccessResponse();

        var user = await userRepository.GetByEmailAddressAsync(
            request.Email,
            cancellationToken);

        // Prevent user enumeration.
        if (user is null)
        {
            return response;
        }

        // Remove any existing password reset tokens.
        await passwordResetTokenRepository.DeleteByUserIdAsync(
            user.UserId,
            cancellationToken);

        // Generate secure token.
        var resetToken = tokenGenerator.GeneratePasswordResetToken();

        // Hash before storing.
        var tokenHash = tokenHasher.Hash(resetToken);

        var passwordResetToken = PasswordResetToken.Create(
            userId: user.UserId,
            tokenHash: tokenHash,
            expiresAt: DateTimeOffset.UtcNow.AddMinutes(PasswordResetTokenExpiryInMinutes));

        await passwordResetTokenRepository.AddAsync(
            passwordResetToken,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish notification after successful commit.
        await mediator.Publish(
            new PasswordResetRequestedEvent(
                user.UserId,
                user.FullName,
                user.EmailAddress,
                resetToken),
            cancellationToken);

        return response;
    }

    private static ApiResponse<ForgotPasswordResponse> CreateSuccessResponse()
    {
        return ApiResponse<ForgotPasswordResponse>.SuccessResponse(
            new ForgotPasswordResponse
            {
                Message = "If an account exists for the provided email address, a password reset email has been sent."
            });
    }
}