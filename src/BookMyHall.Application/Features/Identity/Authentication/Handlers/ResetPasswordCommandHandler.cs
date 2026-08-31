using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Application.Features.Identity.Authentication;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Messaging;

namespace BookMyHall.Application.Features.Authentication.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    ITokenHasher tokenHasher,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    IMessagePublisher messagePublisher)
    : IRequestHandler<ResetPasswordCommand, ApiResponse<ResetPasswordResponse>>
{
    public async Task<ApiResponse<ResetPasswordResponse>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAddressAsync
        (
            request.Email,
            cancellationToken
        );

        if (user is null)
        {
            return ApiResponse<ResetPasswordResponse>.FailureResponse
            (
                "Invalid or expired password reset link.",
                HttpStatusCode.BadRequest
            );
        }

        var tokenHash = tokenHasher.Hash(request.Token);

        var resetToken = await passwordResetTokenRepository.GetActiveTokenAsync
        (
            user.UserId,
            tokenHash,
            cancellationToken
        );

        if (resetToken is null)
        {
            return ApiResponse<ResetPasswordResponse>.FailureResponse
            (
                "Invalid or expired password reset link.",
                HttpStatusCode.BadRequest
            );
        }

        var passwordHash = passwordHasher.HashPassword(request.NewPassword);

        user.UpdatePassword(passwordHash);

        await userRepository.UpdateAsync
        (
            user,
            cancellationToken
        );

        await passwordResetTokenRepository.DeleteByUserIdAsync
        (
            user.UserId,
            cancellationToken
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var passwordResetSuccessMessage = new PasswordResetSuccessMessage
        (
            user.UserId,
            user.FullName,
            user.EmailAddress
        );

        await messagePublisher.PublishAsync
        (
            passwordResetSuccessMessage,
            cancellationToken
        );

        return ApiResponse<ResetPasswordResponse>.SuccessResponse
        (
            new ResetPasswordResponse
            {
                Message = "Password has been reset successfully."
            },
            "Password has been reset successfully."
        );
    }
}