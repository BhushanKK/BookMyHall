using System.Net;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Options;

using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Infrastructure.Authentication;
using BookMyHall.Shared.Common;

namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserSessionRepository userSessionRepository,
    IUnitOfWork unitOfWork,
    IJwtTokenService jwtTokenService,
    IValidator<RefreshTokenCommand> validator,
    IMessageHelper messageHelper,
    IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<RefreshTokenCommand, ApiResponse<LoginResponse>>
{
    public async Task<ApiResponse<LoginResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // Validate Request
        // ---------------------------------------------------------

        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<LoginResponse>.FailureResponse
            (
                string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );
        }

        // ---------------------------------------------------------
        // Load Refresh Token + Required User Data
        // ---------------------------------------------------------

        var refreshToken = await refreshTokenRepository.GetByTokenAsync(
            request.RefreshToken,
            cancellationToken);

        if (refreshToken is null)
        {
            return ApiResponse<LoginResponse>.FailureResponse
            (
                messageHelper.InvalidRefreshToken(),
                HttpStatusCode.Unauthorized
            );
        }

        // ---------------------------------------------------------
        // Validate Refresh Token
        // ---------------------------------------------------------

        if (refreshToken.IsRevoked)
        {
            return ApiResponse<LoginResponse>.FailureResponse
            (
                messageHelper.InvalidRefreshToken(),
                HttpStatusCode.Unauthorized
            );
        }

        if (refreshToken.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            return ApiResponse<LoginResponse>.FailureResponse
            (
                messageHelper.RefreshTokenExpired(),
                HttpStatusCode.Unauthorized
            );
        }

        // ---------------------------------------------------------
        // Validate User
        // ---------------------------------------------------------

        if (!refreshToken.IsActive)
        {
            return ApiResponse<LoginResponse>.FailureResponse
            (
                messageHelper.UserInactive(),
                HttpStatusCode.Forbidden
            );
        }

        // ---------------------------------------------------------
        // Generate Access Token
        // ---------------------------------------------------------

        var jwtResult = jwtTokenService.GenerateToken(
            new JwtUser
            {
                UserId = refreshToken.UserId,
                FullName = refreshToken.FullName,
                MobileNumber = refreshToken.MobileNumber,
                EmailAddress = refreshToken.EmailAddress,
                TokenVersion = refreshToken.TokenVersion,
                Roles = refreshToken.Roles
            });

        // ---------------------------------------------------------
        // Revoke Old Refresh Token
        // ---------------------------------------------------------

        await refreshTokenRepository.RevokeAsync
        (
            refreshToken.RefreshTokenId,
            refreshToken.UserId,
            cancellationToken
        );

        // ---------------------------------------------------------
        // Create New Refresh Token
        // ---------------------------------------------------------

        var newRefreshTokenValue = jwtTokenService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            RefreshTokenId = Guid.NewGuid(),
            UserId = refreshToken.UserId,
            Token = newRefreshTokenValue,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryDays),
            CreatedBy = refreshToken.UserId
        };

        await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        // ---------------------------------------------------------
        // Update User Session
        // ---------------------------------------------------------

        var session = await userSessionRepository.GetByRefreshTokenIdAsync(refreshToken.RefreshTokenId, cancellationToken);

        if (session is not null)
        {
            session.RefreshTokenId = newRefreshToken.RefreshTokenId;
            session.LastActivity = DateTimeOffset.UtcNow;
            await userSessionRepository.UpdateAsync(session, cancellationToken);
        }
        // ---------------------------------------------------------
        // Persist Changes
        // ---------------------------------------------------------
        await unitOfWork.SaveChangesAsync(cancellationToken);
        // ---------------------------------------------------------
        // Prepare Response
        // ---------------------------------------------------------

        var response = new LoginResponse
        {
            UserId = refreshToken.UserId,
            FullName = refreshToken.FullName,
            MobileNumber = refreshToken.MobileNumber,
            EmailAddress = refreshToken.EmailAddress,
            Roles = refreshToken.Roles,

            AccessToken = jwtResult.AccessToken,
            RefreshToken = newRefreshTokenValue,
            ExpiresAt = jwtResult.ExpiresAt
        };
        return ApiResponse<LoginResponse>.SuccessResponse
        (
            response,
            messageHelper.LoginSuccessful(),
            HttpStatusCode.OK
        );
    }
}