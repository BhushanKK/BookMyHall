using MediatR;
using BookMyHall.Application.Features.Identity.Users;
using BookMyHall.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using BookMyHall.Domain.Enums;
using BookMyHall.Domain.Dtos;
namespace BookMyHall.Api.Endpoints.Identity;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .DisableAntiforgery();

        group.MapPost("/", async (
            SignupUserCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("SignUp User/Customer")
        .WithSummary("SignUp feature for User/customer")
        .WithDescription("SignUp a new user.")
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/createuser", async (
            CreateUserCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("Create New User")
        .WithSummary("Create New User")
        .WithDescription("Creates a new user.")
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<UserDto>>(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status401Unauthorized);

            group.MapPut(
            "/{userId:guid}",
            async (
                Guid userId,
                [FromForm] UpdateUserForm form,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                Stream? imageStream = null;

                try
                {
                    if (form.Image is not null && form.Image.Length > 0)
                        imageStream = form.Image.OpenReadStream();

                    var command = new ProfileUpdateUserCommand(
                        UserId: userId,
                        FirstName: form.FirstName,
                        MiddleName: form.MiddleName,
                        LastName: form.LastName,
                        MobileNumber: form.MobileNumber,
                        DateOfBirth: form.DateOfBirth,
                        Gender: form.Gender,
                        EmailAddress: form.EmailAddress,
                        ImageStream: imageStream,
                        FileName: form.Image?.FileName,
                        ContentType: form.Image?.ContentType,
                        FileSize: form.Image?.Length
                    );

                    var response = await mediator.Send(command, cancellationToken);
                    return Results.Json(response, statusCode: (int)response.StatusCode);
                }
                finally
                {
                    if (imageStream is not null)
                        await imageStream.DisposeAsync();
                }
            })
            .DisableAntiforgery()
            .WithName("UpdateUser")
            .WithSummary("Update User")
            .WithDescription("Updates user information and optionally uploads a profile image.")
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

public sealed class UpdateUserForm
{
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public DateTimeOffset? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
}