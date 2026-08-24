using MediatR;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.MenuRolePermission;

public static class MenuRolePermissionEndpoints
{
    public static void MapMenuRolePermissionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/menu-role-permissions")
            .WithTags("Menu Role Permission")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (CreateMenuRolePermissionCommand command,
        IMediator mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreateMenuRolePermission")
        .WithSummary("Create Menu Role Permission")
        .WithDescription("Assigns a menu to a role.")
        .Produces<ApiResponse<MenuRolePermissionDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{menuRolePermissionId:guid}", async (
            Guid menuRolePermissionId,
            UpdateMenuRolePermissionCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.MenuRolePermissionId = menuRolePermissionId;
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdateMenuRolePermission")
        .WithSummary("Update Menu Role Permission")
        .WithDescription("Updates a menu-role permission mapping.")
        .Produces<ApiResponse<MenuRolePermissionDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{menuRolePermissionId:guid}",
            async (Guid menuRolePermissionId,
                IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send
                (
                    new DeleteMenuRolePermissionCommand(menuRolePermissionId),
                    cancellationToken
                );

                return Results.Json(response, statusCode: response.StatusCode);
            })
        .WithName("DeleteMenuRolePermission")
        .WithSummary("Delete Menu Role Permission")
        .WithDescription("Deletes a menu-role permission mapping.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{menuRolePermissionId:guid}",
            async (
                Guid menuRolePermissionId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send
                (
                    new GetMenuRolePermissionByIdQuery(menuRolePermissionId),
                    cancellationToken
                );

                return Results.Json(response, statusCode: response.StatusCode);
            })
        .WithName("GetMenuRolePermissionById")
        .WithSummary("Get Menu Role Permission By Id")
        .WithDescription("Returns a menu-role permission mapping by its identifier.")
        .Produces<ApiResponse<MenuRolePermissionDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            int pageNumber,
            int pageSize,
            string? searchText,
            string? sortBy,
            bool sortDescending,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var paginationRequest =
                new PaginationRequest
                {
                    PageNumber = pageNumber <= 0
                        ? 1
                        : pageNumber,

                    PageSize = pageSize <= 0
                        ? 10
                        : pageSize,

                    SearchText = searchText,
                    SortBy = sortBy,
                    SortDescending = sortDescending
                };

            var response = await mediator.Send
            (
                new GetMenuRolePermissionQuery(paginationRequest),
                    cancellationToken
            );

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetMenuRolePermissions")
        .WithSummary("Get Menu Role Permissions")
        .WithDescription("Returns paginated menu-role permission mappings.")
        .Produces<ApiResponse<PaginatedResponse<MenuRolePermissionDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);
    }
}