using MediatR;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Identity;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/roles")
            .RequireAuthorization();

        group.MapPost("/", async (
            CreateRoleCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command,cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/{roleId:guid}", async (Guid roleId,
            UpdateRoleCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.RoleId=roleId;
            var result = await mediator.Send(command,cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/{roleId:guid}", async (
            Guid roleId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteRoleCommand(roleId),cancellationToken);
            return Results.Ok(result);
        });

        group.MapGet("/{roleId:guid}", async (
            Guid roleId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetRoleByIdQuery(roleId),cancellationToken);
            return Results.Ok(result);
        });

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetRolesQuery(request),cancellationToken);
            return Results.Ok(result);
        });
    }
}