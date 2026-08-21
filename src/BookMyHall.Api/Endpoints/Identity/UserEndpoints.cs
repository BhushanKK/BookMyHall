using MediatR;
using BookMyHall.Application.Features.Identity.Users;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Enums;
namespace BookMyHall.Api.Endpoints.Identity;

public static class UserEndpoints
{

    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .DisableAntiforgery()
            .RequireAuthorization();

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
        string firstName,
        string? middleName,
        string? lastName,
        string mobileNumber,
        DateTimeOffset? dateOfBirth,
        Gender? gender,
        string emailAddress,
        Guid roleId,
        string? profileImageUrl,
        IFormFile? image,
        IMediator mediator,
        CancellationToken cancellationToken) =>
    {
        Stream? imageStream = null;

        try
        {
            if (image is not null && image.Length > 0)
            {
                imageStream = image.OpenReadStream();
            }

            var command = new UpdateUserCommand(
                UserId: userId,
                FirstName: firstName,
                MiddleName: middleName,
                LastName: lastName,
                MobileNumber: mobileNumber,
                DateOfBirth: dateOfBirth,
                Gender: gender,
                EmailAddress: emailAddress,
                RoleId: roleId,
                profileImageUrl: profileImageUrl,
                ImageStream: imageStream,
                FileName: image?.FileName,
                ContentType: image?.ContentType,
                FileSize: image?.Length);

            var response = await mediator.Send(command,cancellationToken);

            return Results.Json(response,statusCode: (int)response.StatusCode);
        }
        finally
        {
            if (imageStream is not null)
            {
                await imageStream.DisposeAsync();
            }
        }
    })
    .DisableAntiforgery()
    .Accepts<IFormFile>("multipart/form-data")
    .WithName("UpdateUser")
    .WithSummary("Update User")
    .WithDescription(
        "Updates user profile information and optionally uploads a profile picture.")
    .Produces<ApiResponse<UserDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status409Conflict)
    .RequireAuthorization();

        group.MapDelete("/{userId:guid}", async (
            Guid userId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new DeleteUserCommand(userId), cancellationToken);
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
            var response = await mediator.Send(new GetUsersQuery(request), cancellationToken);
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
            var response = await mediator.Send(new GetUserByIdQuery(userId), cancellationToken);
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