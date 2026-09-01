using System.Net;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Features.Identity.Authentication;

namespace BookMyHall.Application.Features.Authentication.Commands.SetPassword;

public sealed class SetPasswordCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IValidator<SetPasswordCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        SetPasswordCommand,
        ApiResponse<SetPasswordResponse>>
{
    public async Task<ApiResponse<SetPasswordResponse>> Handle(
        SetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // ------------------------------------------------------------
        // 1. Validate request
        // ------------------------------------------------------------

        var validationResult =
            await validator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse(
                string.Join(
                    " | ",
                    validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest);
        }

        // ------------------------------------------------------------
        // 2. Get user
        // ------------------------------------------------------------

        var user =
            await userRepository.GetByIdAsync(
                request.UserId,
                cancellationToken);

        if (user is null)
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.User),
                HttpStatusCode.NotFound);
        }

        // ------------------------------------------------------------
        // 3. User must have verified email
        // ------------------------------------------------------------

        if (!user.IsEmailVerified)
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse(
                "Please verify your email address before setting your password.",
                HttpStatusCode.BadRequest);
        }

        // ------------------------------------------------------------
        // 4. User must be active
        // ------------------------------------------------------------

        if (!user.IsActive)
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse(
                messageHelper.UserInactive(),
                HttpStatusCode.Forbidden);
        }

        // ------------------------------------------------------------
        // 5. Password must not already be configured
        // ------------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse(
                "Password has already been configured.",
                HttpStatusCode.BadRequest);
        }

        // ------------------------------------------------------------
        // 6. Hash password
        // ------------------------------------------------------------

        var passwordHash =
            passwordHasher.HashPassword(
                request.NewPassword);

        // ------------------------------------------------------------
        // 7. Update password
        // ------------------------------------------------------------

        user.UpdatePassword(passwordHash);

        var now = DateTimeOffset.UtcNow;

        user.UpdatedBy = user.UserId;
        user.UpdatedDate = now;

        // ------------------------------------------------------------
        // 8. Save user
        // ------------------------------------------------------------

        await userRepository.UpdateAsync(
            user,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // ------------------------------------------------------------
        // 10. Clear user cache
        // ------------------------------------------------------------

        await cacheService.RemoveByPrefixAsync(
            $"{CacheKeys.UsersPaged}:",
            cancellationToken);

        // ------------------------------------------------------------
        // 11. Response
        // ------------------------------------------------------------

        var response = new SetPasswordResponse
        {
            Message = "Your password has been set successfully.",
            UserId = user.UserId,
            PasswordSet = true
        };

        return ApiResponse<SetPasswordResponse>.SuccessResponse
        (
            response,
            "Password set successfully.",
            HttpStatusCode.OK
        );
    }
}