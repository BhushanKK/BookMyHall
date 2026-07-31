using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Infrastructure.Authentication;

namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
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
        // Validate Request
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<LoginResponse>.FailureResponse
            (
                string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );
        }

        // Load Refresh Token
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken,cancellationToken);

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

        // Generate new JWT
        var jwtResult = jwtTokenService.GenerateToken(
            new JwtUser
            {
                UserId = user.UserId,
                FullName = user.FullName,
                MobileNumber = user.MobileNumber,
                EmailAddress = user.EmailAddress,
                Roles = user.UserRoles
                    .Select(x => x.Role.RoleName)
                    .ToList()
            });

        // Rotate Refresh Token
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTimeOffset.UtcNow;
        refreshToken.RevokedBy = user.UserId;

        await refreshTokenRepository.UpdateAsync(
            refreshToken,
            cancellationToken);

        var newRefreshTokenValue = jwtTokenService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            UserId = user.UserId,
            Token = newRefreshTokenValue,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryDays),
            CreatedBy = user.UserId
        };

        await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Response
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