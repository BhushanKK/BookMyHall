using MediatR;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.MenuRolePermission;

public static class MenuRolePermissionEndpoints
{
    public static void MapMenuRolePermissionEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/menu-role-permissions")
            .WithTags("Menu Role Permission")
            .RequireAuthorization();

        group.MapGet("/role/{roleId:guid}",
            async (Guid roleId, IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetMenuRolePermissionsByRoleIdQuery(roleId), cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithName("GetMenuRolePermissionsByRoleId")
            .WithSummary("Get Menu Permissions By Role")
            .WithDescription("Returns all menu permissions assigned to a role.")
            .Produces<ApiResponse<IReadOnlyList<MenuRolePermissionDto>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapPost( "/upsert",
            async (UpsertMenuRolePermissionCommand command,IMediator mediator,
                    CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(command, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithName("UpsertMenuRolePermissions")
            .WithSummary("Upsert Menu Permissions")
            .WithDescription("Creates or updates menu permissions for a role.")
            .Produces<ApiResponse<IReadOnlyList<MenuRolePermissionDto>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);
    }
}