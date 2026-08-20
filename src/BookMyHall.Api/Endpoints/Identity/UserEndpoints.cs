using MediatR;
using BookMyHall.Application.Features.Identity.Users;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Identity;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users");

        group.MapPost("/", async (
            CreateUserCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreateUser")
        .WithSummary("Create User")
        .WithDescription("Creates a new user.")
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("/{userId:guid}", async (
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
                request.DateOfBirth,
                request.Gender,
                request.EmailAddress,
                request.RoleId);

            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdateUser")
        .WithSummary("Update User")
        .WithDescription("Updates an existing user.")
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status401Unauthorized).RequireAuthorization();

        group.MapDelete("/{userId:guid}", async (
            Guid userId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new DeleteUserCommand(userId),cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteUser")
        .WithSummary("Delete User")
        .WithDescription("Deletes an existing user.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized).RequireAuthorization();

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetUsersQuery(request),cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetUsers")
        .WithSummary("Get Users")
        .WithDescription("Returns a paginated list of users.")
        .Produces<ApiResponse<PaginatedResponse<UserDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized).RequireAuthorization();

        group.MapGet("/{userId:guid}", async (
            Guid userId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetUserByIdQuery(userId),cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetUserById")
        .WithSummary("Get User By Id")
        .WithDescription("Returns a user by its identifier.")
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized).RequireAuthorization();

        return app;
    }
}