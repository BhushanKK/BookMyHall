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
    public async Task<ApiResponse<bool>> Handle
    (
        ChangePasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync
        (
            request,
            cancellationToken
        );

        if (!validationResult.IsValid)
        {
            return ApiResponse<bool>.FailureResponse
            (
                string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );
        }

        if (!currentUser.UserId.HasValue)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.Unauthorized(),
                HttpStatusCode.Unauthorized
            );
        }

        var userId = currentUser.UserId.Value;

        var user = await userRepository.GetByIdAsync
        (
            userId,
            cancellationToken
        );

        if (user is null)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.NotFound
            );
        }

        if (!user.IsActive)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.UserInactive(),
                HttpStatusCode.Forbidden
            );
        }

        var currentPasswordValid = passwordHasher.VerifyPassword
        (
            user.PasswordHash!,
            request.CurrentPassword
        );

        if (!currentPasswordValid)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.PasswordMismatch(),
                HttpStatusCode.BadRequest
            );
        }

        var samePassword = passwordHasher.VerifyPassword
        (
            user.PasswordHash!,
            request.NewPassword
        );

        if (samePassword)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.PasswordAlreadyUsed(),
                HttpStatusCode.BadRequest
            );
        }

        var newPasswordHash = passwordHasher.HashPassword
        (
            request.NewPassword
        );

        user.UpdatePassword(newPasswordHash);

        var now = DateTimeOffset.UtcNow;

        user.UpdatedBy = userId;
        user.UpdatedDate = now;

        await refreshTokenRepository.RevokeAllByUserIdAsync
        (
            userId,
            cancellationToken
        );

        await userSessionRepository.EndAllSessionsAsync
        (
            userId,
            cancellationToken
        );

        await userRepository.UpdateAsync
        (
            user,
            cancellationToken
        );

        await unitOfWork.SaveChangesAsync
        (
            cancellationToken
        );

        var passwordChangedMessage = new PasswordChangedMessage
        (
            user.UserId,
            user.FullName,
            user.EmailAddress
        );

        await messagePublisher.PublishAsync
        (
            passwordChangedMessage,
            cancellationToken
        );

        await cacheService.RemoveByPrefixAsync
        (
            $"{CacheKeys.UsersPaged}:",
            cancellationToken
        );

        return ApiResponse<bool>.SuccessResponse
        (
            true,
            messageHelper.PasswordChangedSuccessfully(),
            HttpStatusCode.OK
        );
    }
}