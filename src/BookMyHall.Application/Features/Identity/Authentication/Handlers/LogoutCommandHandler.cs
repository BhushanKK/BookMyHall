using System.Net;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;

namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class LogoutCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserSessionRepository userSessionRepository,
    IUnitOfWork unitOfWork,
    IValidator<LogoutCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<LogoutCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<bool>.FailureResponse
            (
                string.Join(" | ",
                    validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );
        }

        // Load refresh token
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(
            request.RefreshToken,
            cancellationToken);

        if (refreshToken is null || refreshToken.IsRevoked)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.InvalidRefreshToken(),
                HttpStatusCode.Unauthorized
            );
        }

        // Revoke refresh token
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTimeOffset.UtcNow;
        refreshToken.RevokedBy = refreshToken.UserId;
        await refreshTokenRepository.RevokeAsync(refreshToken.RefreshTokenId, refreshToken.UserId, cancellationToken);

        // End active session
        var session = await userSessionRepository.GetByRefreshTokenIdAsync(refreshToken.RefreshTokenId, cancellationToken);

        if (session is not null && session.IsActive)
        {
            session.IsActive = false;
            session.SessionEnd = DateTimeOffset.UtcNow;
            session.LastActivity = DateTimeOffset.UtcNow;
            await userSessionRepository.UpdateAsync(session, cancellationToken);
        }

        // Commit
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse
        (
            true,
            messageHelper.LogoutSuccessful(),
            HttpStatusCode.OK
        );
    }
}