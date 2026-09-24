using System.Net;

using MediatR;
using AutoMapper;

using BookMyHall.Contracts.Common;
using BookMyHall.Application.Features.Identity.Users;
using BookMyHall.Application.Features.Identity.Authentication;
using BookMyHall.Application.Features.Authentication.Commands.ForgotPassword;
using BookMyHall.Application.Features.Authentication.Commands.ResetPassword;
using BookMyHall.Application.Features.Authentication.Commands.VerifyEmail;
using BookMyHall.Application.Features.Authentication.Commands.ResendVerificationEmail;
using BookMyHall.Contracts.Authentication;
using BookMyHall.Application.Features.Authentication;
using BookMyHall.Domain.Identity;
using BookMyHall.Application.Features.Identity;

namespace BookMyHall.Api.Endpoints.Identity;

public static class AuthenticationEndpoints
{
    private const string RefreshTokenCookieName = "bookmyhall_refresh_token";
    private const string RefreshTokenCookiePath = "/api/authentication";

    public static void MapAuthenticationEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group =
            app.MapGroup("/api/authentication")
                .WithTags("Authentication");

        group.MapPost("/login", async (
            LoginRequest request,
            IMapper mapper,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var command =
                mapper.Map<LoginCommand>(request);

            var response =
                await mediator.Send(
                    command,
                    cancellationToken);

            if (response.StatusCode == (int)HttpStatusCode.OK &&
                response.Data is not null &&
                !string.IsNullOrWhiteSpace(
                    response.Data.RefreshToken))
            {
                SetRefreshTokenCookie(
                    httpContext,
                    response.Data.RefreshToken);

                response.Data.RefreshToken =
                    string.Empty;
            }

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("Login")
        .WithSummary("User Login")
        .WithDescription(
            "Authenticates a user and returns an access token.")
        .Produces<ApiResponse<LoginResponse>>(
            StatusCodes.Status200OK)
        .Produces<ApiResponse<LoginResponse>>(
            StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<LoginResponse>>(
            StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh-token", async (
            HttpContext httpContext,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var refreshToken =
                httpContext.Request.Cookies[
                    RefreshTokenCookieName];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Results.Json(
                    ApiResponse<LoginResponse>.FailureResponse(
                        "Refresh token cookie not found.",
                        HttpStatusCode.BadRequest),
                    statusCode:
                        StatusCodes.Status400BadRequest);
            }

            var command =
                new RefreshTokenCommand(
                    refreshToken);

            var response =
                await mediator.Send(
                    command,
                    cancellationToken);

            if (response.StatusCode == (int)HttpStatusCode.OK &&
                response.Data is not null &&
                !string.IsNullOrWhiteSpace(
                    response.Data.RefreshToken))
            {
                SetRefreshTokenCookie(
                    httpContext,
                    response.Data.RefreshToken);

                response.Data.RefreshToken =
                    string.Empty;
            }

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("RefreshToken")
        .WithSummary("Refresh Access Token")
        .WithDescription(
            "Generates a new access token using a valid refresh token.")
        .Produces<ApiResponse<LoginResponse>>(
            StatusCodes.Status200OK)
        .Produces<ApiResponse<LoginResponse>>(
            StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<LoginResponse>>(
            StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", async (
            HttpContext httpContext,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            if (!httpContext.Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) 
                || string.IsNullOrWhiteSpace(refreshToken))
            {
                DeleteRefreshTokenCookie(httpContext);
                return Results.Json
                (
                    ApiResponse<bool>.SuccessResponse(true, "Logout successful.", 
                    HttpStatusCode.OK
                ),
                statusCode: StatusCodes.Status200OK);
            }

            var command = new LogoutCommand(refreshToken);
            var response = await mediator.Send(command, cancellationToken);
            DeleteRefreshTokenCookie(httpContext);
            return Results.Json(response, statusCode: response.StatusCode);
        }).AllowAnonymous();

        group.MapPost("/change-password", async (
            ChangePasswordCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response =
                await mediator.Send(
                    command,
                    cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .RequireAuthorization()
        .WithName("ChangePassword")
        .WithSummary("Change Password")
        .WithDescription(
            "Changes the password of the currently authenticated user.")
        .Produces<ApiResponse<bool>>(
            StatusCodes.Status200OK)
        .Produces<ApiResponse<bool>>(
            StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<bool>>(
            StatusCodes.Status401Unauthorized);

        group.MapPost("/forgot-password", async (
            ForgotPasswordCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response =
                await mediator.Send(
                    command,
                    cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .AllowAnonymous()
        .WithName("ForgotPassword")
        .WithSummary("Forgot Password")
        .WithDescription(
            "Generates a password reset token for the specified email address.")
        .Produces<ApiResponse<ForgotPasswordResponse>>(
            StatusCodes.Status200OK)
        .Produces<ApiResponse<ForgotPasswordResponse>>(
            StatusCodes.Status400BadRequest);

        group.MapPost("/reset-password", async (
            ResetPasswordCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response =
                await mediator.Send(
                    command,
                    cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .AllowAnonymous()
        .WithName("ResetPassword")
        .WithSummary("Reset Password")
        .WithDescription(
            "Resets the user's password using a valid password reset token.")
        .Produces<ApiResponse<ResetPasswordResponse>>(
            StatusCodes.Status200OK)
        .Produces<ApiResponse<ResetPasswordResponse>>(
            StatusCodes.Status400BadRequest);

        group.MapPost("/verify-email", async (
            VerifyEmailCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response =
                await mediator.Send(
                    command,
                    cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .AllowAnonymous()
        .WithName("VerifyEmail")
        .WithSummary("Verify Email")
        .WithDescription(
            "Verifies the user's email address using a valid email verification token.")
        .Produces<ApiResponse<VerifyEmailResponse>>(
            StatusCodes.Status200OK)
        .Produces<ApiResponse<VerifyEmailResponse>>(
            StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<VerifyEmailResponse>>(
            StatusCodes.Status404NotFound);

        group.MapPost("/resend-verification-email", async (
            ResendVerificationEmailCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response =
                await mediator.Send(
                    command,
                    cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .AllowAnonymous()
        .WithName("ResendVerificationEmail")
        .WithSummary("Resend Verification Email")
        .WithDescription(
            "Resends an email verification link if the email address is registered and not yet verified.")
        .Produces<ApiResponse<ResendVerificationEmailResponse>>(
            StatusCodes.Status200OK)
        .Produces<ApiResponse<ResendVerificationEmailResponse>>(
            StatusCodes.Status400BadRequest);

        group.MapPost("/google-login", async (
            GoogleLoginRequest request,
            ISender sender,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var command =
                new GoogleLoginCommand(
                    request.Credential,
                    request.DeviceIdentifier,
                    request.DeviceName,
                    request.PushNotificationToken,
                    request.AppVersion);

            var response = await sender.Send(command, cancellationToken);

            if (response.StatusCode == (int)HttpStatusCode.OK &&
                response.Data is not null &&
                !string.IsNullOrWhiteSpace(
                    response.Data.RefreshToken))
            {
                SetRefreshTokenCookie(
                    httpContext,
                    response.Data.RefreshToken);

                response.Data.RefreshToken =
                    string.Empty;
            }

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .AllowAnonymous()
        .WithName("GoogleLogin")
        .WithSummary("Google Login")
        .WithDescription(
            "Authenticates a user using Google and returns an access token.")
        .Produces<ApiResponse<LoginResponse>>(
            StatusCodes.Status200OK)
        .Produces<ApiResponse<LoginResponse>>(
            StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<LoginResponse>>(
            StatusCodes.Status401Unauthorized);

        group.MapGet(
            "/user-details/{userId:guid}/{roleId:guid}",
            async (
                Guid userId,
                Guid roleId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var response =
                    await mediator.Send(
                        new GetUserDetailsByIdQuery(
                            userId,
                            roleId),
                        cancellationToken);

                return Results.Json(
                    response,
                    statusCode: response.StatusCode);
            })
            .WithName("GetUserDetailsById")
            .WithSummary("Get User Details By Id")
            .WithDescription(
                "Returns the details of a user by user identifier and role identifier.")
            .Produces<ApiResponse<UserDetailsView>>(
                StatusCodes.Status200OK)
            .Produces(
                StatusCodes.Status401Unauthorized)
            .Produces(
                StatusCodes.Status404NotFound);
    }

    private static void SetRefreshTokenCookie(HttpContext httpContext, string refreshToken)
    {
        httpContext.Response.Cookies.Append(RefreshTokenCookieName, refreshToken,
        new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = RefreshTokenCookiePath,
            MaxAge = TimeSpan.FromDays(30)
        });
    }

    private static void DeleteRefreshTokenCookie(
        HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = RefreshTokenCookiePath
            });
    }
}