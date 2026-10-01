using System.Net;
using MediatR;
using FluentValidation;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Features.Identity.Authentication;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;

namespace BookMyHall.Application.Features.Authentication.Commands.SetPassword;

public sealed class SetPasswordCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IValidator<SetPasswordCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<SetPasswordCommand, ApiResponse<SetPasswordResponse>>
{
    public async Task<ApiResponse<SetPasswordResponse>> Handle(
        SetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse
            (
                string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.User), 
                HttpStatusCode.NotFound
            );
        }

        if (!user.IsEmailVerified)
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse
            (
                "Please verify your email address before setting your password.",
                HttpStatusCode.BadRequest
            );
        }

        if (!user.IsActive)
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse
            (
                messageHelper.UserInactive(),
                HttpStatusCode.Forbidden
            );
        }

        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse
            (
                "Password has already been configured.",
                HttpStatusCode.BadRequest
            );
        }

        var passwordHash = passwordHasher.HashPassword(request.NewPassword);

        user.UpdatePassword(passwordHash);
        var now = DateTimeOffset.UtcNow;
        user.UpdatedBy = user.UserId;
        user.UpdatedDate = now;

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.RemoveByPrefixAsync($"{CacheKeys.UsersPaged}:", cancellationToken);

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