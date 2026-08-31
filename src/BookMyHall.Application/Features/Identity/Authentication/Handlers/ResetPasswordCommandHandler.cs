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
        // 1. Find user by UserId from reset URL
        var user = await userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            return ApiResponse<ResetPasswordResponse>.FailureResponse(
                "Invalid or expired password reset link.",
                HttpStatusCode.BadRequest);
        }

        // 2. Hash the token received from the reset URL
        var tokenHash = tokenHasher.Hash(request.Token);

        // 3. Find active reset token
        var resetToken = await passwordResetTokenRepository.GetActiveTokenAsync(
            user.UserId,
            tokenHash,
            cancellationToken);

        if (resetToken is null)
        {
            return ApiResponse<ResetPasswordResponse>.FailureResponse(
                "Invalid or expired password reset link.",
                HttpStatusCode.BadRequest);
        }

        // 4. Hash the new password
        var passwordHash = passwordHasher.HashPassword(
            request.NewPassword);

        // 5. Update password
        user.UpdatePassword(passwordHash);

        await userRepository.UpdateAsync(
            user,
            cancellationToken);

        // 6. Delete reset token so it cannot be reused
        await passwordResetTokenRepository.DeleteByUserIdAsync(
            user.UserId,
            cancellationToken);

        // 7. Commit transaction
        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // 8. Publish password reset success message
        var passwordResetSuccessMessage = new PasswordResetSuccessMessage(
            user.UserId,
            user.FullName,
            user.EmailAddress);

        await messagePublisher.PublishAsync(
            passwordResetSuccessMessage,
            cancellationToken);

        // 9. Return success
        return ApiResponse<ResetPasswordResponse>.SuccessResponse(
            new ResetPasswordResponse
            {
                Message = "Password has been reset successfully."
            },
            "Password has been reset successfully.");
    }
}