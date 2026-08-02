using System.Net;

using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Options;

using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;
using BookMyHall.Infrastructure.Authentication;
using BookMyHall.Shared.Common;

namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUserSessionRepository userSessionRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IValidator<LoginCommand> validator,
    IMessageHelper messageHelper,
    IMapper mapper,
    IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
{
    public async Task<ApiResponse<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // Validate Request
        // ---------------------------------------------------------

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return ApiResponse<LoginResponse>.FailureResponse
            (
                string.Join(" | ",
                    validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );
        }
        // ---------------------------------------------------------
        // Load User
        // ---------------------------------------------------------
        var user = await userRepository.GetForLoginAsync(request.MobileNumber, cancellationToken);

        if (user is null)
        {
            return ApiResponse<LoginResponse>.FailureResponse
            (
                messageHelper.InvalidCredentials(),
                HttpStatusCode.Unauthorized
            );
        }

        // ---------------------------------------------------------
        // Verify Password
        // ---------------------------------------------------------

        if (!passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            return ApiResponse<LoginResponse>.FailureResponse
            (
                messageHelper.InvalidCredentials(),
                HttpStatusCode.Unauthorized
            );
        }

        // ---------------------------------------------------------
        // Verify User Status
        // ---------------------------------------------------------

        if (!user.IsActive)
        {
            return ApiResponse<LoginResponse>.FailureResponse
            (
                messageHelper.UserInactive(),
                HttpStatusCode.Forbidden
            );
        }
        // ---------------------------------------------------------
        // Update Login Information
        // ---------------------------------------------------------
        user.RecordLogin();
        user.UpdatedBy = user.UserId;
        user.UpdatedDate = DateTimeOffset.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        // ---------------------------------------------------------
        // Generate JWT Access Token
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
        // Generate Refresh Token
        // ---------------------------------------------------------

        var refreshTokenValue = jwtTokenService.GenerateRefreshToken();
        var refreshToken = new RefreshToken
        {
            RefreshTokenId = Guid.NewGuid(),
            UserId = user.UserId,
            Token = refreshTokenValue,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryDays),
            CreatedBy = user.UserId
        };

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        // ---------------------------------------------------------
        // Create User Session
        // ---------------------------------------------------------

        var now = DateTimeOffset.UtcNow;

        var userSession = new UserSession
        {
            UserSessionId = Guid.NewGuid(),
            UserId = user.UserId,
            RefreshTokenId = refreshToken.RefreshTokenId,
            DeviceId = null,
            SessionStart = now,
            LastActivity = now,
            IsActive = true,
            CreatedBy = user.UserId
        };

        await userSessionRepository.AddAsync(userSession, cancellationToken);
        // ---------------------------------------------------------
        // Persist Everything
        // ---------------------------------------------------------
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // ---------------------------------------------------------
        // Prepare Response
        // ---------------------------------------------------------
        var response = mapper.Map<LoginResponse>(user);
        response.AccessToken = jwtResult.AccessToken;
        response.RefreshToken = refreshTokenValue;
        response.ExpiresAt = jwtResult.ExpiresAt;
        return ApiResponse<LoginResponse>.SuccessResponse
        (
            response,
            messageHelper.LoginSuccessful(),
            HttpStatusCode.OK
        );
    }
}