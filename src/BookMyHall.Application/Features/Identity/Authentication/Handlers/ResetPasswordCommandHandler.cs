using MediatR;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Features.Identity.Authentication;
using BookMyHall.Application.Features.Authentication.Events;

namespace BookMyHall.Application.Features.Authentication.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    ITokenHasher tokenHasher,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    IMediator mediator)
    : IRequestHandler<ResetPasswordCommand, ApiResponse<ResetPasswordResponse>>
{
    public async Task<ApiResponse<ResetPasswordResponse>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // Find user
        var user = await userRepository.GetByEmailAddressAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return ApiResponse<ResetPasswordResponse>.FailureResponse
            (
                "Invalid or expired password reset link.",
                System.Net.HttpStatusCode.BadRequest
            );
        }

        // Hash incoming token
        var tokenHash = tokenHasher.Hash(request.Token);

        // Verify token
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
                System.Net.HttpStatusCode.BadRequest
            );
        }

        // Hash password
        var passwordHash = passwordHasher.HashPassword(request.NewPassword);

        // Update password
        user.UpdatePassword(passwordHash);
        await userRepository.UpdateAsync(user, cancellationToken);

        // Remove all password reset tokens for this user
        await passwordResetTokenRepository.DeleteByUserIdAsync(user.UserId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await mediator.Publish(
        new PasswordResetCompletedEvent(
        user.UserId,
        user.FullName,
        user.EmailAddress),
        cancellationToken);
        
        return ApiResponse<ResetPasswordResponse>.SuccessResponse(new ResetPasswordResponse { Message = "Password has been reset successfully." }, "Password has been reset successfully.");
    }
}