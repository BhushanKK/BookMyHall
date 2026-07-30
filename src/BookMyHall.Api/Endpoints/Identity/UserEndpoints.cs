using MediatR;
using BookMyHall.Application.Features.Identity.Users;

namespace BookMyHall.Api.Endpoints.Identity;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(
        this IEndpointRouteBuilder app)
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
        return app;
    }
}