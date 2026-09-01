using System.Net;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Messaging;
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
    IMessageHelper messageHelper,
    IMessagePublisher messagePublisher,
    ICacheService cacheService)
    : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        // ------------------------------------------------------------
        // 1. Validate request
        // ------------------------------------------------------------

        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<bool>.FailureResponse(
                string.Join(
                    " | ",
                    validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest);
        }

        // ------------------------------------------------------------
        // 2. Get current user
        // ------------------------------------------------------------

        if (!currentUser.UserId.HasValue)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.Unauthorized(),
                HttpStatusCode.Unauthorized);
        }

        var userId = currentUser.UserId.Value;

        // ------------------------------------------------------------
        // 3. Get user from database
        // ------------------------------------------------------------

        var user = await userRepository.GetByIdAsync(
            userId,
            cancellationToken);

        if (user is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.User),
                HttpStatusCode.NotFound);
        }

        // ------------------------------------------------------------
        // 4. Check active status
        // ------------------------------------------------------------

        if (!user.IsActive)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.UserInactive(),
                HttpStatusCode.Forbidden);
        }

        // ------------------------------------------------------------
        // 5. Email verification is required
        // ------------------------------------------------------------

        if (!user.IsEmailVerified)
        {
            return ApiResponse<bool>.FailureResponse(
                "Please verify your email address before changing your password.",
                HttpStatusCode.Forbidden);
        }

        // ------------------------------------------------------------
        // 6. Validate current password
        // ------------------------------------------------------------

        var currentPasswordValid = passwordHasher.VerifyPassword(
            user.PasswordHash!,
            request.CurrentPassword);

        if (!currentPasswordValid)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.PasswordMismatch(),
                HttpStatusCode.BadRequest);
        }

        // ------------------------------------------------------------
        // 7. Prevent using the same password
        // ------------------------------------------------------------

        var samePassword = passwordHasher.VerifyPassword(
            user.PasswordHash!,
            request.NewPassword);

        if (samePassword)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.PasswordAlreadyUsed(),
                HttpStatusCode.BadRequest);
        }

        // ------------------------------------------------------------
        // 8. Hash new password
        // ------------------------------------------------------------

        var newPasswordHash = passwordHasher.HashPassword(
            request.NewPassword);

        // ------------------------------------------------------------
        // 9. Update password
        // ------------------------------------------------------------

        user.UpdatePassword(newPasswordHash);

        var now = DateTimeOffset.UtcNow;

        user.UpdatedBy = userId;
        user.UpdatedDate = now;

        // ------------------------------------------------------------
        // 10. Revoke existing refresh tokens
        // ------------------------------------------------------------

        await refreshTokenRepository.RevokeAllByUserIdAsync(
            userId,
            cancellationToken);

        // ------------------------------------------------------------
        // 11. End existing sessions
        // ------------------------------------------------------------

        await userSessionRepository.EndAllSessionsAsync(
            userId,
            cancellationToken);

        // ------------------------------------------------------------
        // 12. Update user
        // ------------------------------------------------------------

        await userRepository.UpdateAsync(
            user,
            cancellationToken);

        // ------------------------------------------------------------
        // 13. Save database changes
        // ------------------------------------------------------------

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // ------------------------------------------------------------
        // 14. Publish Password Changed event/message
        // ------------------------------------------------------------

        var passwordChangedMessage = new PasswordChangedMessage(
            user.UserId,
            user.FullName,
            user.EmailAddress);

        await messagePublisher.PublishAsync(
            passwordChangedMessage,
            cancellationToken);

        // ------------------------------------------------------------
        // 15. Clear user cache
        // ------------------------------------------------------------

        await cacheService.RemoveByPrefixAsync(
            $"{CacheKeys.UsersPaged}:",
            cancellationToken);

        // ------------------------------------------------------------
        // 16. Return success
        // ------------------------------------------------------------

        return ApiResponse<bool>.SuccessResponse(
            true,
            messageHelper.PasswordChangedSuccessfully(),
            HttpStatusCode.OK);
    }
}