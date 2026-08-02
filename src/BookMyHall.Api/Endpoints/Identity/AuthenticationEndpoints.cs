using MediatR;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Features.Identity.Users;
using BookMyHall.Application.Features.Identity.Authentication;

namespace BookMyHall.Api.Endpoints.Identity;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/authentication")
            .WithTags("Authentication");

        group.MapPost("/login", async (
            LoginRequest request,
            IMapper mapper,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<LoginCommand>(request);
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("Login")
        .WithSummary("User Login")
        .WithDescription("Authenticates a user and returns an access token and refresh token.")
        .Produces<ApiResponse<LoginResponse>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<LoginResponse>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<LoginResponse>>(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh-token", async (
            RefreshTokenRequest request,
            IMapper mapper,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<RefreshTokenCommand>(request);
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("RefreshToken")
        .WithSummary("Refresh Access Token")
        .WithDescription("Generates a new access token using a valid refresh token.")
        .Produces<ApiResponse<LoginResponse>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<LoginResponse>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<LoginResponse>>(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", async (
            LogoutRequest request,
            IMapper mapper,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<LogoutCommand>(request);
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .RequireAuthorization()
        .WithName("Logout")
        .WithSummary("Logout User")
        .WithDescription("Revokes the refresh token and logs the current user out.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<bool>>(StatusCodes.Status401Unauthorized);

        group.MapPost("/change-password", async (
            ChangePasswordCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .RequireAuthorization()
        .WithName("ChangePassword")
        .WithSummary("Change Password")
        .WithDescription("Changes the password of the currently authenticated user.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<bool>>(StatusCodes.Status401Unauthorized);
    }
}