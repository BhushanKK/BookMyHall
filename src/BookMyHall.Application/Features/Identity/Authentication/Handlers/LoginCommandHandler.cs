using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Application.Common.Interfaces.Storage;
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
    IDeviceRepository deviceRepository,
    IR2StorageService storageService)
    : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
{
    public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<LoginResponse>.FailureResponse
            (
                string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage)),
                HttpStatusCode.BadRequest
            );
        }

        var user = await userRepository.GetForLoginAsync(request.MobileNumber, cancellationToken);
        
        if (user is null)
            return ApiResponse<LoginResponse>.FailureResponse(messageHelper.InvalidCredentials(), HttpStatusCode.Unauthorized);
        
        if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
        {
            user.ProfileImageUrl = await storageService.GetPreSignedUrlAsync(
                user.ProfileImageUrl,
                TimeSpan.FromDays(7),
                cancellationToken);
        }
        if (!passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            await RecordFailedLoginAsync(user.UserId, cancellationToken);
            return ApiResponse<LoginResponse>.FailureResponse(messageHelper.InvalidCredentials(), HttpStatusCode.Unauthorized);
        }

        var now = DateTimeOffset.UtcNow;
        await userRepository.RecordLoginAsync(user.UserId, now, cancellationToken);
        var jwtResult = jwtTokenService.GenerateToken(new JwtUser
        {
            UserId = user.UserId,
            FullName = user.FullName,
            MobileNumber = user.MobileNumber,
            EmailAddress = user.EmailAddress,
            TokenVersion = user.TokenVersion,
            Roles = user.Roles
        });

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
        var device = await GetOrCreateDeviceAsync(user.UserId,request, now, cancellationToken);

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
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var loginHistory = new UserLoginHistory
        {
            UserLoginHistoryId = Guid.NewGuid(),
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
            LoginSource = clientInfoService.LoginSource ?? "Unknown",
            IsMfaUsed = false
        };

        await userLoginHistoryRepository.AddAsync(
            loginHistory,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<LoginResponse>(user);

        response.AccessToken = jwtResult.AccessToken;
        response.RefreshToken = refreshTokenValue;
        response.ExpiresAt = jwtResult.ExpiresAt;
        return ApiResponse<LoginResponse>.SuccessResponse(response, messageHelper.LoginSuccessful(),HttpStatusCode.OK);
    }

    private async Task RecordFailedLoginAsync(Guid userId, CancellationToken cancellationToken)
    {
        var failedLoginHistory = new UserLoginHistory
        {
            UserLoginHistoryId = Guid.NewGuid(),
            UserId = userId,
            SessionId = null,
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
        };

        await userLoginHistoryRepository.AddAsync(failedLoginHistory, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Device> GetOrCreateDeviceAsync( Guid userId, LoginCommand request,
        DateTimeOffset now, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.GetByDeviceIdentifierAsync(userId, request.DeviceIdentifier, cancellationToken);

        if (device is null)
        {
            device = new Device
            {
                DeviceId = Guid.NewGuid(),
                UserId = userId,
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
            return device;
        }

        device.PushNotificationToken = request.PushNotificationToken;
        device.DeviceName = request.DeviceName;
        device.DeviceType = clientInfoService.DeviceType ?? "Desktop";
        device.OperatingSystem = clientInfoService.OperatingSystem;
        device.Browser = clientInfoService.Browser;
        device.AppVersion = request.AppVersion;
        device.LastIpAddress = clientInfoService.IpAddress;
        device.LastLoginDate = now;
        device.LastActivity = now;
        device.UpdatedDate = now;
        await deviceRepository.UpdateAsync(device, cancellationToken);

        return device;
    }
}