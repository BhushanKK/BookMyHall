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
using BookMyHall.Domain.Audit;
using BookMyHall.Domain.Common;
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
    IOptions<JwtOptions> jwtOptions,
    IUserLoginHistoryRepository userLoginHistoryRepository,
    IClientInfoService clientInfoService,
    IDeviceRepository deviceRepository)
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
                string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );
        }

        // ---------------------------------------------------------
        // Load User Login Information
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
            await userLoginHistoryRepository.AddAsync(
                new UserLoginHistory
                {
                    UserId = user.UserId,
                    LoginDate = DateTimeOffset.UtcNow,
                    LoginStatus = LoginStatuses.Failed,
                    LoginMethod = LoginMethods.Password,
                    FailureReason = "Invalid password.",
                    IpAddress = clientInfoService.IpAddress ?? "Unknown",
                    UserAgent = clientInfoService.UserAgent ?? "Unknown",
                    Browser = clientInfoService.Browser ?? "Unknown",
                    OperatingSystem = clientInfoService.OperatingSystem ?? "Unknown",
                    DeviceType = clientInfoService.DeviceType ?? "Unknown",
                    LoginSource = clientInfoService.LoginSource ?? "Unknown",
                    IsMfaUsed = false
                },
                cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<LoginResponse>.FailureResponse
            (
                messageHelper.InvalidCredentials(),
                HttpStatusCode.Unauthorized
            );
        }

        // ---------------------------------------------------------
        // Update Login Information
        // ---------------------------------------------------------

        var now = DateTimeOffset.UtcNow;
        await userRepository.RecordLoginAsync(user.UserId, now, cancellationToken);

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
                Roles = user.Roles
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
            ExpiresAt = now.AddDays(jwtOptions.Value.RefreshTokenExpiryDays),
            CreatedBy = user.UserId
        };

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        // ---------------------------------------------------------
        // Register / Update Device
        // ---------------------------------------------------------

        var device =
            await deviceRepository.GetByDeviceIdentifierAsync(
                user.UserId,
                request.DeviceIdentifier,
                cancellationToken);

        if (device is null)
        {
            device = new Device
            {
                DeviceId = Guid.NewGuid(),
                UserId = user.UserId,
                DeviceIdentifier = request.DeviceIdentifier,
                PushNotificationToken = request.PushNotificationToken,
                DeviceName = request.DeviceName,
                DeviceType = clientInfoService.DeviceType ?? "Desktop",
                OperatingSystem = clientInfoService.OperatingSystem,
                Browser = clientInfoService.Browser,
                AppVersion = request.AppVersion,
                LastIpAddress = clientInfoService.IpAddress,
                LastLoginDate = now,
                LastActivity = now,
                IsTrusted = false,
                IsActive = true,
                CreatedDate = now
            };

            await deviceRepository.AddAsync(device, cancellationToken);
        }
        else
        {
            device.PushNotificationToken = request.PushNotificationToken;
            device.DeviceName =request.DeviceName;
            device.DeviceType = clientInfoService.DeviceType ?? "Desktop";
            device.OperatingSystem =clientInfoService.OperatingSystem;
            device.Browser = clientInfoService.Browser;
            device.AppVersion = request.AppVersion;
            device.LastIpAddress = clientInfoService.IpAddress;
            device.LastLoginDate = now;
            device.LastActivity = now;
            device.UpdatedDate = now;
            await deviceRepository.UpdateAsync(device, cancellationToken);
        }

        // ---------------------------------------------------------
        // Create User Session
        // ---------------------------------------------------------

        var userSession = new UserSession
        {
            UserSessionId = Guid.NewGuid(),
            UserId = user.UserId,
            RefreshTokenId = refreshToken.RefreshTokenId,
            DeviceId = device.DeviceId,
            SessionStart = now,
            LastActivity = now,
            IsActive = true,
            CreatedBy = user.UserId
        };

        await userSessionRepository.AddAsync(userSession, cancellationToken);

        // ---------------------------------------------------------
        // Persist Refresh Token, Device and Session
        // ---------------------------------------------------------

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // ---------------------------------------------------------
        // Add Successful Login History
        // ---------------------------------------------------------

        await userLoginHistoryRepository.AddAsync(
            new UserLoginHistory
            {
                UserId = user.UserId,
                SessionId = userSession.UserSessionId,
                LoginDate = now,
                LoginStatus = LoginStatuses.Success,
                LoginMethod = LoginMethods.Password,
                IpAddress = clientInfoService.IpAddress ?? "Unknown",
                UserAgent = clientInfoService.UserAgent ?? "Unknown",
                Browser = clientInfoService.Browser ?? "Unknown",
                OperatingSystem = clientInfoService.OperatingSystem ?? "Unknown",
                DeviceType = clientInfoService.DeviceType ?? "Unknown",
                LoginSource = clientInfoService.LoginSource
            },
            cancellationToken);

        // ---------------------------------------------------------
        // Persist Login History
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