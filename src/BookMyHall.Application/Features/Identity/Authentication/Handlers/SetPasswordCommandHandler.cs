using System.Net;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Features.Identity.Authentication;
using BookMyHall.Application.Abstractions.Security;

namespace BookMyHall.Application.Features.Authentication.Commands.SetPassword;

public sealed class SetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    IValidator<SetPasswordCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<SetPasswordCommand, ApiResponse<SetPasswordResponse>>
{
    public async Task<ApiResponse<SetPasswordResponse>> Handle(
        SetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // ------------------------------------------------------------
        // 1. Validate request
        // ------------------------------------------------------------

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<SetPasswordResponse>.FailureResponse
            (
                message,
                HttpStatusCode.BadRequest
            );
        }

        // ------------------------------------------------------------
        // 2. Get user
        // ------------------------------------------------------------

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.NotFound
            );
        }

        // ------------------------------------------------------------
        // 3. Email must be verified first
        // ------------------------------------------------------------

        if (!user.IsEmailVerified)
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse
            (
                "Please verify your email address before setting your password.",
                HttpStatusCode.BadRequest
            );
        }

        // ------------------------------------------------------------
        // 4. Make sure password has not already been configured
        // ------------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return ApiResponse<SetPasswordResponse>.FailureResponse
            (
                "Password has already been configured.",
                HttpStatusCode.BadRequest
            );
        }

        // ------------------------------------------------------------
        // 5. Hash password
        // ------------------------------------------------------------

        var passwordHash = passwordHasher.HashPassword(request.NewPassword);

        // ------------------------------------------------------------
        // 6. Update password
        // ------------------------------------------------------------

        user.UpdatePassword(passwordHash);

        // ------------------------------------------------------------
        // 7. Save changes
        // ------------------------------------------------------------

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // ------------------------------------------------------------
        // 8. Return response
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