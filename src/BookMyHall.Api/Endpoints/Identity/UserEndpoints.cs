using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Features.Identity.Users;

namespace BookMyHall.Api.Endpoints.Identity;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users");

        group.MapPost("/", async (CreateUserCommand command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(command, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
        .WithName("CreateUser")
        .Produces<UserDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{userId:guid}",async (
            Guid userId,
            UpdateUserRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateUserCommand(
                userId,
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.MobileNumber,
                request.EmailAddress,
                request.RoleId);
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("UpdateUser")
        .WithSummary("Update User")
        .WithDescription("Updates an existing user.")
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{userId:guid}", async (Guid userId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new DeleteUserCommand(userId),
                cancellationToken);

            return Results.Json(
                response,
                statusCode: (int)response.StatusCode);
        })
        .WithName("DeleteUser")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async ([AsParameters] PaginationRequest request,
                IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetUsersQuery(request), cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
        .WithName("GetUsers")
        .Produces<ApiResponse<PaginatedResponse<UserDto>>>(StatusCodes.Status200OK);

        group.MapGet("/{userId:guid}",
            async (
                Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(
                    new GetUserByIdQuery(userId),
                    cancellationToken);

                return Results.Json(
                    response,
                    statusCode: (int)response.StatusCode);
            })
        .WithName("GetUserById")
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status404NotFound);

        return app;
    }
}