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
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<bool>.FailureResponse(
                string.Join(
                    " | ",
                    validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest);
        }

        // ---------------------------------------------------------
        // Load Refresh Token
        // ---------------------------------------------------------

        var refreshToken =
            await refreshTokenRepository.GetByTokenAsync(
                request.RefreshToken,
                cancellationToken);

        if (refreshToken is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.InvalidRefreshToken(),
                HttpStatusCode.Unauthorized);
        }

        // ---------------------------------------------------------
        // Check Refresh Token Status
        // ---------------------------------------------------------

        if (refreshToken.IsRevoked)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.InvalidRefreshToken(),
                HttpStatusCode.Unauthorized);
        }

        // ---------------------------------------------------------
        // Revoke Refresh Token
        // ---------------------------------------------------------
        // TryRevokeAsync performs the revoke operation atomically.
        // Only the request that successfully changes the token
        // from active -> revoked will receive true.

        var tokenRevoked =
            await refreshTokenRepository.TryRevokeAsync(
                refreshToken.RefreshTokenId,
                refreshToken.UserId,
                cancellationToken);

        if (!tokenRevoked)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.InvalidRefreshToken(),
                HttpStatusCode.Unauthorized);
        }

        // ---------------------------------------------------------
        // End Active User Session
        // ---------------------------------------------------------

        var session =
            await userSessionRepository.GetByRefreshTokenIdAsync(
                refreshToken.RefreshTokenId,
                cancellationToken);

        if (session is not null && session.IsActive)
        {
            var now = DateTimeOffset.UtcNow;

            session.IsActive = false;
            session.SessionEnd = now;
            session.LastActivity = now;

            await userSessionRepository.UpdateAsync(
                session,
                cancellationToken);
        }

        // ---------------------------------------------------------
        // Persist Changes
        // ---------------------------------------------------------

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // ---------------------------------------------------------
        // Response
        // ---------------------------------------------------------

        return ApiResponse<bool>.SuccessResponse(
            true,
            messageHelper.LogoutSuccessful(),
            HttpStatusCode.OK);
    }
}