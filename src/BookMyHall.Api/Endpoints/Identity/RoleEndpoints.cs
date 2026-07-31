using MediatR;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Role;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/roles")
        .WithTags("Role")
        .RequireAuthorization();

        group.MapPost("/", async (
            CreateRoleCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);

            return TypedResults.Ok(result);
        })
        .WithName("CreateRole")
        .WithSummary("Create Role")
        .WithDescription("Creates a new role.")
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{roleId:guid}", async (Guid roleId,
            UpdateRoleCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.RoleId=roleId;
            var result = await mediator.Send(command,cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName("UpdateRole")
        .WithSummary("Update Role")
        .WithDescription("Updates an existing role.")
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{roleId:guid}", async (
            Guid roleId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteRoleCommand(roleId),cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName("DeleteRole")
        .WithSummary("Delete Role")
        .WithDescription("Deletes an existing role.")
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{roleId:guid}", async (
            Guid roleId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetRoleByIdQuery(roleId),cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName("GetRoleById")
        .WithSummary("Get Roles By Id")
        .WithDescription("Returns a role by its identifier.");

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetRolesQuery(request),cancellationToken);
            return TypedResults.Ok(result);
        })
        .WithName("GetRoles")
        .WithSummary("Get Roles")
        .WithDescription("Returns all roles.");
    }
}