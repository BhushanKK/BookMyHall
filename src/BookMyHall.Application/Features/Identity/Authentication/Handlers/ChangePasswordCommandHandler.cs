using System.Net;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Application.Features.Authentication.Events;
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
    IMessageHelper messageHelper,
    IMediator mediator)
    : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // Validate Request
        // ---------------------------------------------------------

        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<bool>.FailureResponse
            (
                string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );
        }

        // ---------------------------------------------------------
        // Check Authentication
        // ---------------------------------------------------------

        if (!currentUser.UserId.HasValue)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.Unauthorized(),
                HttpStatusCode.Unauthorized
            );
        }

        var userId = currentUser.UserId.Value;

        // ---------------------------------------------------------
        // Load User
        // ---------------------------------------------------------

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.NotFound
            );
        }

        // ---------------------------------------------------------
        // Check User Status
        // ---------------------------------------------------------

        if (!user.IsActive)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.UserInactive(),
                HttpStatusCode.Forbidden
            );
        }

        // ---------------------------------------------------------
        // Verify Current Password
        // ---------------------------------------------------------

        var currentPasswordValid = passwordHasher.VerifyPassword(user.PasswordHash, request.CurrentPassword);

        if (!currentPasswordValid)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.PasswordMismatch(),
                HttpStatusCode.BadRequest
            );
        }

        // ---------------------------------------------------------
        // Prevent Reusing Current Password
        // ---------------------------------------------------------

        var samePassword =passwordHasher.VerifyPassword(user.PasswordHash,request.NewPassword);

        if (samePassword)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.PasswordAlreadyUsed(),
                HttpStatusCode.BadRequest
            );
        }

        // ---------------------------------------------------------
        // Hash New Password
        // ---------------------------------------------------------

        var newPasswordHash = passwordHasher.HashPassword(request.NewPassword);

        // ---------------------------------------------------------
        // Update Password
        // ---------------------------------------------------------

        user.UpdatePassword(newPasswordHash);

        var now = DateTimeOffset.UtcNow;

        user.UpdatedBy = userId;
        user.UpdatedDate = now;

        // ---------------------------------------------------------
        // Revoke All Refresh Tokens
        // ---------------------------------------------------------

        await refreshTokenRepository.RevokeAllByUserIdAsync(userId, cancellationToken);

        // ---------------------------------------------------------
        // End All Active Sessions
        // ---------------------------------------------------------

        await userSessionRepository.EndAllSessionsAsync(userId, cancellationToken);

        // ---------------------------------------------------------
        // Update User
        // ---------------------------------------------------------

        await userRepository.UpdateAsync(user, cancellationToken);

        // ---------------------------------------------------------
        // Commit Transaction
        // ---------------------------------------------------------

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // ---------------------------------------------------------
        // Publish Password Changed Event
        // ---------------------------------------------------------

        await mediator.Publish
        (
            new PasswordChangedEvent(user.UserId, user.FullName, user.EmailAddress),
            cancellationToken
        );

        // ---------------------------------------------------------
        // Return Response
        // ---------------------------------------------------------

        return ApiResponse<bool>.SuccessResponse
        (
            true,
            messageHelper.PasswordChangedSuccessfully(),
            HttpStatusCode.OK
        );
    }
}