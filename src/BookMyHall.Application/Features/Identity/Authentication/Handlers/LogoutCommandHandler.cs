using MediatR;
using System.Net;
using FluentValidation;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class LogoutCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IValidator<LogoutCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<LogoutCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<bool>.FailureResponse
            (
                string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );
        }

        var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (refreshToken is null)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.InvalidRefreshToken(),
                HttpStatusCode.Unauthorized
            );
        }

        if (refreshToken.IsRevoked)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.InvalidRefreshToken(),
                HttpStatusCode.Unauthorized
            );
        }

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTimeOffset.UtcNow;
        refreshToken.RevokedBy = refreshToken.UserId;

        await refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse
        (
            true,
            messageHelper.LogoutSuccessful(),
            HttpStatusCode.OK
        );
    }
}