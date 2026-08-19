using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Api.Endpoints.Identity;

public static class RolePermissionEndpoints
{
    public static void MapRolePermissionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/roles")
            .WithTags("Role Permission")
            .RequireAuthorization( policy => policy.RequireRole("Admin"));

        MapAssignPermission(group);
        MapGetRolePermissions(group);
        MapRemovePermission(group);
    }

    private static void MapAssignPermission(
        RouteGroupBuilder group)
    {
        group.MapPost("/{roleId:guid}/permissions/{permissionId:guid}",
            async (Guid roleId,Guid permissionId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command =new AssignRolePermissionCommand(roleId,permissionId);
                var response = await mediator.Send(command,cancellationToken);

                return Results.Json(response,statusCode: response.StatusCode);
            })
            .WithName("AssignPermissionToRole")
            .WithSummary("Assign Permission To Role")
            .WithDescription("Assigns a permission to a role.")
            .Produces<ApiResponse<RolePermissionDto>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static void MapGetRolePermissions(RouteGroupBuilder group)
    {
        group.MapGet("/{roleId:guid}/permissions",
            async (Guid roleId,IMediator mediator,CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetRolePermissionsQuery(roleId),cancellationToken);
                return Results.Json(response,statusCode: response.StatusCode);
            })
            .WithName("GetRolePermissions")
            .WithSummary("Get Role Permissions")
            .WithDescription("Returns all permissions assigned to a role.")
            .Produces<ApiResponse<IReadOnlyList<RolePermissionDto>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static void MapRemovePermission( RouteGroupBuilder group)
    {
        group.MapDelete("/{roleId:guid}/permissions/{permissionId:guid}",
            async ( Guid roleId,Guid permissionId,IMediator mediator,CancellationToken cancellationToken) =>
            {
                var response =await mediator.Send(new RemoveRolePermissionCommand(roleId, permissionId),cancellationToken);
                return Results.Json(response,statusCode: response.StatusCode);
            })
            .WithName("RemovePermissionFromRole")
            .WithSummary("Remove Permission From Role")
            .WithDescription("Removes a permission from a role.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
    }
}