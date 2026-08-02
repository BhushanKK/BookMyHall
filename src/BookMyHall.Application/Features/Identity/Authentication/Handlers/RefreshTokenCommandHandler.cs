using System.Net;
using AutoMapper;
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
    IMapper mapper,
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
        // Load Refresh Token
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

        var user = refreshToken.User;

        if (!user.IsActive)
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
                UserId = user.UserId,
                FullName = user.FullName,
                MobileNumber = user.MobileNumber,
                EmailAddress = user.EmailAddress,
                TokenVersion = user.TokenVersion,
                Roles = user.UserRoles
                    .Select(x => x.Role.RoleName)
                    .ToList()
            });

        // ---------------------------------------------------------
        // Revoke Old Refresh Token
        // ---------------------------------------------------------

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTimeOffset.UtcNow;
        refreshToken.RevokedBy = user.UserId;

        await refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

        // ---------------------------------------------------------
        // Create New Refresh Token
        // ---------------------------------------------------------

        var newRefreshTokenValue = jwtTokenService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            RefreshTokenId = Guid.NewGuid(),
            UserId = user.UserId,
            Token = newRefreshTokenValue,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(
                jwtOptions.Value.RefreshTokenExpiryDays),
            CreatedBy = user.UserId
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
        var response = mapper.Map<LoginResponse>(user);
        response.AccessToken = jwtResult.AccessToken;
        response.RefreshToken = newRefreshTokenValue;
        response.ExpiresAt = jwtResult.ExpiresAt;
        return ApiResponse<LoginResponse>.SuccessResponse
        (
            response,
            messageHelper.LoginSuccessful(),
            HttpStatusCode.OK
        );
    }
}