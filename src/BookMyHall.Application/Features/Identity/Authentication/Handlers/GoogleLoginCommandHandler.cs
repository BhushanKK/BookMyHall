using System.Net;
using AutoMapper;
using Google.Apis.Auth;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Application.Features.Identity.Authentication;

using BookMyHall.Contracts.Common;

using BookMyHall.Domain.Audit;
using BookMyHall.Domain.Common;
using BookMyHall.Domain.Dtos;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Domain.Identity;

using BookMyHall.Infrastructure.Authentication;

using BookMyHall.Persistence.Exceptions;

using BookMyHall.Shared.Common;
using BookMyHall.Shared.Configuration;

namespace BookMyHall.Application.Features.Authentication;

public sealed class GoogleLoginCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUserSessionRepository userSessionRepository,
    IUnitOfWork unitOfWork,
    IJwtTokenService jwtTokenService,
    IMessageHelper messageHelper,
    IMapper mapper,
    IOptions<JwtOptions> jwtOptions,
    IOptions<GoogleOptions> googleOptions,
    IUserLoginHistoryRepository userLoginHistoryRepository,
    IClientInfoService clientInfoService,
    IDeviceRepository deviceRepository,
    IR2StorageService storageService,
    ILogger<GoogleLoginCommandHandler> logger)
    : IRequestHandler<GoogleLoginCommand, ApiResponse<LoginResponse>>
{
    public async Task<ApiResponse<LoginResponse>> Handle(
        GoogleLoginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // ============================================================
            // 1. Validate request
            // ============================================================

            if (string.IsNullOrWhiteSpace(request.Credential))
            {
                return ApiResponse<LoginResponse>.FailureResponse(
                    "Google credential is required.",
                    HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(request.DeviceIdentifier))
            {
                return ApiResponse<LoginResponse>.FailureResponse(
                    "Device identifier is required.",
                    HttpStatusCode.BadRequest);
            }

            // ============================================================
            // 2. Validate Google configuration
            // ============================================================

            var clientId = googleOptions.Value.ClientId;

            if (string.IsNullOrWhiteSpace(clientId))
            {
                logger.LogError(
                    "Google authentication is not configured. Google ClientId is missing.");

                return ApiResponse<LoginResponse>.FailureResponse(
                    "Google authentication is not configured.",
                    HttpStatusCode.InternalServerError);
            }

            // ============================================================
            // 3. Validate Google ID token
            // ============================================================

            GoogleJsonWebSignature.Payload payload;

            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(
                    request.Credential,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = [clientId]
                    });
            }
            catch (InvalidJwtException ex)
            {
                logger.LogWarning(
                    ex,
                    "Invalid Google authentication token.");

                return ApiResponse<LoginResponse>.FailureResponse(
                    "Invalid Google authentication.",
                    HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error while validating Google authentication token.");

                return ApiResponse<LoginResponse>.FailureResponse(
                    "Unable to validate Google authentication.",
                    HttpStatusCode.Unauthorized);
            }

            // ============================================================
            // 4. Validate Google email
            // ============================================================

            if (string.IsNullOrWhiteSpace(payload.Email))
            {
                return ApiResponse<LoginResponse>.FailureResponse(
                    "Google account email is not available.",
                    HttpStatusCode.Unauthorized);
            }

            if (!payload.EmailVerified)
            {
                return ApiResponse<LoginResponse>.FailureResponse(
                    "Google email address is not verified.",
                    HttpStatusCode.Unauthorized);
            }

            var emailAddress =
                payload.Email.Trim().ToLowerInvariant();

            logger.LogInformation(
                "Google login requested for {EmailAddress}.",
                emailAddress);

            // ============================================================
            // 5. Find existing BookMyHall user
            // ============================================================

            var user =
                await userRepository.GetForGoogleLoginAsync(
                    emailAddress,
                    cancellationToken);

            // ============================================================
            // 6. Create Google user if user does not exist
            // ============================================================

            if (user is null)
            {
                logger.LogInformation(
                    "Google user does not exist. Creating user for {EmailAddress}.",
                    emailAddress);

                user = await CreateGoogleUserAsync(
                    payload,
                    emailAddress,
                    cancellationToken);

                if (user is null)
                {
                    logger.LogError(
                        "Google user could not be created or retrieved for {EmailAddress}.",
                        emailAddress);

                    return ApiResponse<LoginResponse>.FailureResponse(
                        "Unable to create Google user.",
                        HttpStatusCode.InternalServerError);
                }

                logger.LogInformation(
                    "Google user created successfully. UserId: {UserId}",
                    user.UserId);
            }
            else
            {
                logger.LogInformation(
                    "Existing BookMyHall user found. UserId: {UserId}",
                    user.UserId);
            }

            // ============================================================
            // 7. Get profile image URL
            // ============================================================

            if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
            {
                user.ProfileImageUrl =
                    await storageService.GetPreSignedUrlAsync(
                        user.ProfileImageUrl,
                        TimeSpan.FromDays(6).Add(
                            TimeSpan.FromHours(23)),
                        cancellationToken);
            }

            logger.LogInformation(
                "Google login profile processing completed for UserId: {UserId}.",
                user.UserId);

            // ============================================================
            // 8. Record successful login
            // ============================================================

            var now = DateTimeOffset.UtcNow;

            await userRepository.RecordLoginAsync(
                user.UserId,
                now,
                cancellationToken);

            logger.LogInformation(
                "RecordLoginAsync completed for UserId: {UserId}.",
                user.UserId);

            // ============================================================
            // 9. Generate BookMyHall JWT
            // ============================================================

            var jwtResult =
                jwtTokenService.GenerateToken(
                    new JwtUser
                    {
                        UserId = user.UserId,
                        FullName = user.FullName,
                        MobileNumber = user.MobileNumber,
                        EmailAddress = user.EmailAddress,
                        TokenVersion = user.TokenVersion,
                        Roles = user.Roles
                    });

            logger.LogInformation(
                "JWT generated successfully for UserId: {UserId}.",
                user.UserId);

            // ============================================================
            // 10. Generate refresh token
            // ============================================================

            var refreshTokenValue =
                jwtTokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = refreshTokenValue,
                ExpiresAt =
                    now.AddDays(
                        jwtOptions.Value.RefreshTokenExpiryDays),
                CreatedBy = user.UserId
            };

            await refreshTokenRepository.AddAsync(
                refreshToken,
                cancellationToken);

            logger.LogInformation(
                "Refresh token added for UserId: {UserId}.",
                user.UserId);

            // ============================================================
            // 11. Get or create device
            // ============================================================

            var device =
                await GetOrCreateDeviceAsync(
                    user.UserId,
                    request,
                    now,
                    cancellationToken);

            logger.LogInformation(
                "Device processing completed. DeviceId: {DeviceId}, UserId: {UserId}.",
                device.DeviceId,
                user.UserId);

            // ============================================================
            // 12. Create user session
            // ============================================================

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

            await userSessionRepository.AddAsync(
                userSession,
                cancellationToken);

            logger.LogInformation(
                "User session added. SessionId: {SessionId}, UserId: {UserId}.",
                userSession.UserSessionId,
                user.UserId);

            // ============================================================
            // 13. Save authentication data
            // ============================================================

            await unitOfWork.SaveChangesAsync(
                cancellationToken);

            logger.LogInformation(
                "Authentication data saved successfully for UserId: {UserId}.",
                user.UserId);

            // ============================================================
            // 14. Record login history
            // ============================================================

            var loginHistory = new UserLoginHistory
            {
                UserLoginHistoryId = Guid.NewGuid(),
                UserId = user.UserId,
                SessionId = userSession.UserSessionId,
                LoginDate = now,
                LoginStatus = LoginStatuses.Success,
                LoginMethod = LoginMethods.Google,
                IpAddress =
                    clientInfoService.IpAddress
                    ?? "Unknown",
                UserAgent =
                    clientInfoService.UserAgent
                    ?? "Unknown",
                Browser =
                    clientInfoService.Browser
                    ?? "Unknown",
                OperatingSystem =
                    clientInfoService.OperatingSystem
                    ?? "Unknown",
                DeviceType =
                    clientInfoService.DeviceType
                    ?? "Unknown",
                LoginSource =
                    clientInfoService.LoginSource
                    ?? "Unknown",
                IsMfaUsed = false
            };

            await userLoginHistoryRepository.AddAsync(
                loginHistory,
                cancellationToken);

            logger.LogInformation(
                "Login history added for UserId: {UserId}.",
                user.UserId);

            await unitOfWork.SaveChangesAsync(
                cancellationToken);

            logger.LogInformation(
                "Login history saved successfully for UserId: {UserId}.",
                user.UserId);

            // ============================================================
            // 15. Build response
            // ============================================================

            var response =
                mapper.Map<LoginResponse>(user);

            response.AccessToken =
                jwtResult.AccessToken;

            response.RefreshToken =
                refreshTokenValue;

            response.ExpiresAt =
                jwtResult.ExpiresAt;

            // ============================================================
            // 16. Return successful response
            // ============================================================

            logger.LogInformation(
                "Google login completed successfully for UserId: {UserId}.",
                user.UserId);

            return ApiResponse<LoginResponse>.SuccessResponse(
                response,
                messageHelper.LoginSuccessful(),
                HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Google login failed unexpectedly.");

            throw;
        }
    }

    // ================================================================
    // Create Google User
    // ================================================================

    private async Task<UserLoginDto?> CreateGoogleUserAsync(
        GoogleJsonWebSignature.Payload payload,
        string emailAddress,
        CancellationToken cancellationToken)
    {
        // ============================================================
        // 1. Get Customer role
        // ============================================================

        var roleId =
            await roleRepository.GetRoleIdByRoleName(
                "Customer",
                cancellationToken);

        var customerRole =
            await roleRepository.GetByIdAsync(
                roleId,
                cancellationToken);

        if (customerRole is null)
        {
            logger.LogError(
                "Customer role was not found while creating Google user.");

            return null;
        }

        // ============================================================
        // 2. Create Google user
        // ============================================================

        var currentDate =
            DateTimeOffset.UtcNow;

        var userId =
            Guid.NewGuid();

        var user = new User
        {
            UserId = userId,

            FirstName =
                payload.GivenName
                ?? payload.Name
                ?? "Google",

            LastName =
                payload.FamilyName,

            EmailAddress =
                emailAddress,

            // Google authentication does not provide
            // a BookMyHall mobile number.
            //
            // IMPORTANT:
            // MobileNumber is nullable in the database
            // and has a UNIQUE constraint.
            // NULL is therefore required here.
            MobileNumber = null,

            // Google authentication does not require
            // a local BookMyHall password.
            PasswordHash = null,

            // Google has verified the email address.
            IsEmailVerified = true,

            IsMobileVerified = false,

            IsActive = true,

            IsDeleted = false,

            TokenVersion = 1,

            CreatedDate = currentDate,

            UserRoles =
            [
                new UserRole
                {
                    RoleId =
                        customerRole.RoleId,

                    CreatedDate =
                        currentDate,

                    CreatedBy =
                        userId
                }
            ]
        };

        // ============================================================
        // 3. Save Google user
        // ============================================================

        try
        {
            await userRepository.AddAsync(
                user,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(
                cancellationToken);

            logger.LogInformation(
                "Google user and Customer role saved. UserId: {UserId}, Email: {EmailAddress}.",
                userId,
                emailAddress);
        }
        catch (DuplicateRecordException ex)
        {
            logger.LogWarning(
                ex,
                "Duplicate Google user detected for {EmailAddress}. " +
                "The user may have been created concurrently.",
                emailAddress);

            // Another request may have created
            // the same user concurrently.
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to create Google user. UserId: {UserId}, Email: {EmailAddress}.",
                userId,
                emailAddress);

            throw;
        }

        // ============================================================
        // 4. Reload user with roles
        // ============================================================

        var createdUser =
            await userRepository.GetForGoogleLoginAsync(
                emailAddress,
                cancellationToken);

        if (createdUser is null)
        {
            logger.LogError(
                "Google user was saved but could not be retrieved. Email: {EmailAddress}.",
                emailAddress);

            return null;
        }

        return createdUser;
    }

    // ================================================================
    // Get or Create Device
    // ================================================================

    private async Task<Device> GetOrCreateDeviceAsync(
        Guid userId,
        GoogleLoginCommand request,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var device =
            await deviceRepository.GetByDeviceIdentifierAsync(
                userId,
                request.DeviceIdentifier,
                cancellationToken);

        // ============================================================
        // New device
        // ============================================================

        if (device is null)
        {
            device = new Device
            {
                DeviceId =
                    Guid.NewGuid(),

                UserId =
                    userId,

                DeviceIdentifier =
                    request.DeviceIdentifier,

                PushNotificationToken =
                    request.PushNotificationToken,

                DeviceName =
                    request.DeviceName,

                DeviceType =
                    clientInfoService.DeviceType
                    ?? "Desktop",

                OperatingSystem =
                    clientInfoService.OperatingSystem,

                Browser =
                    clientInfoService.Browser,

                AppVersion =
                    request.AppVersion,

                LastIpAddress =
                    clientInfoService.IpAddress,

                LastLoginDate =
                    now,

                LastActivity =
                    now,

                IsTrusted =
                    false,

                IsActive =
                    true,

                CreatedDate =
                    now
            };

            await deviceRepository.AddAsync(
                device,
                cancellationToken);

            logger.LogInformation(
                "New device added. DeviceId: {DeviceId}, UserId: {UserId}.",
                device.DeviceId,
                userId);

            return device;
        }

        // ============================================================
        // Existing device
        // ============================================================

        device.PushNotificationToken =
            request.PushNotificationToken;

        device.DeviceName =
            request.DeviceName;

        device.DeviceType =
            clientInfoService.DeviceType
            ?? "Desktop";

        device.OperatingSystem =
            clientInfoService.OperatingSystem;

        device.Browser =
            clientInfoService.Browser;

        device.AppVersion =
            request.AppVersion;

        device.LastIpAddress =
            clientInfoService.IpAddress;

        device.LastLoginDate =
            now;

        device.LastActivity =
            now;

        device.UpdatedDate =
            now;

        await deviceRepository.UpdateAsync(
            device,
            cancellationToken);

        logger.LogInformation(
            "Existing device updated. DeviceId: {DeviceId}, UserId: {UserId}.",
            device.DeviceId,
            userId);

        return device;
    }
}