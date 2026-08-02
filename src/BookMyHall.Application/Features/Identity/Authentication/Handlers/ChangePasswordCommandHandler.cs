using System.Net;

using FluentValidation;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUserSessionRepository userSessionRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ICurrentUser currentUser,
    IValidator<ChangePasswordCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Validate Request
        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<bool>.FailureResponse(
                string.Join(" | ",
                    validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest);
        }

        // Check Authentication
        if (!currentUser.UserId.HasValue)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.Unauthorized(),
                HttpStatusCode.Unauthorized);
        }

        // Load User
        var user = await userRepository.GetByIdAsync(
            currentUser.UserId.Value,
            cancellationToken);

        if (user is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.User),
                HttpStatusCode.NotFound);
        }

        // User must be active
        if (!user.IsActive)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.UserInactive(),
                HttpStatusCode.Forbidden);
        }

        // Verify Current Password
        if (!passwordHasher.VerifyPassword(
                user.PasswordHash,
                request.CurrentPassword))
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.PasswordMismatch(),
                HttpStatusCode.BadRequest);
        }

        // Prevent Same Password
        if (passwordHasher.VerifyPassword(
                user.PasswordHash,
                request.NewPassword))
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.PasswordAlreadyUsed(),
                HttpStatusCode.BadRequest);
        }

        // Update Password
        user.UpdatePassword(
            passwordHasher.HashPassword(request.NewPassword));

        // Audit
        user.UpdatedBy = currentUser.UserId;
        user.UpdatedDate = DateTimeOffset.UtcNow;

        // Revoke all refresh tokens
        await refreshTokenRepository.RevokeAllByUserIdAsync(
            user.UserId,
            cancellationToken);

        // End all active sessions
        await userSessionRepository.EndAllSessionsAsync(
            user.UserId,
            cancellationToken);

        // Update user
        await userRepository.UpdateAsync(
            user,
            cancellationToken);

        // Commit Transaction
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(
            true,
            messageHelper.PasswordChangedSuccessfully(),
            HttpStatusCode.OK);
    }
}