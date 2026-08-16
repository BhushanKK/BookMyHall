using MediatR;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Identity;

public static class UserPreferenceEndpoints
{
    public static void MapUserPreferenceEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/user-preferences")
            .WithTags("User Preferences")
            .RequireAuthorization();

        group.MapPost("/", async (
            CreateUserPreferenceCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                command,
                cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("CreateUserPreference")
        .WithSummary("Create User Preference")
        .WithDescription("Creates a new user preference.")
        .Produces<ApiResponse<UserPreferenceDto>>(
            StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{userPreferenceId:guid}", async (
            Guid userPreferenceId,
            UpdateUserPreferenceCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.UserPreferenceId = userPreferenceId;

            var response = await mediator.Send(
                command,
                cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("UpdateUserPreference")
        .WithSummary("Update User Preference")
        .WithDescription("Updates an existing user preference.")
        .Produces<ApiResponse<UserPreferenceDto>>(
            StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{userPreferenceId:guid}", async (
            Guid userPreferenceId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new DeleteUserPreferenceCommand(userPreferenceId),
                cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("DeleteUserPreference")
        .WithSummary("Delete User Preference")
        .WithDescription("Deletes an existing user preference.")
        .Produces<ApiResponse<bool>>(
            StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{userPreferenceId:guid}", async (
            Guid userPreferenceId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetUserPreferenceByIdQuery(userPreferenceId),
                cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("GetUserPreferenceById")
        .WithSummary("Get User Preference By Id")
        .WithDescription(
            "Returns a user preference by its identifier.")
        .Produces<ApiResponse<UserPreferenceDto>>(
            StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);
    }
}