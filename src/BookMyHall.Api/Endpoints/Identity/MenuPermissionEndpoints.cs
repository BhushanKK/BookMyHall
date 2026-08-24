using MediatR;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.MenuPermission;

public static class MenuPermissionEndpoints
{
    public static void MapMenuPermissionEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/menu-permissions")
            .WithTags("Menu Permission")
            .RequireAuthorization();

        group.MapPost("/", async (
            CreateMenuPermissionCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("CreateMenuPermission")
        .WithSummary("Create Menu Permission")
        .WithDescription("Creates a new menu permission.")
        .Produces<ApiResponse<MenuPermissionDto>>(
            StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{menuPermissionId:guid}", async (
            Guid menuPermissionId,
            UpdateMenuPermissionCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.MenuPermissionId = menuPermissionId;
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("UpdateMenuPermission")
        .WithSummary("Update Menu Permission")
        .WithDescription("Updates an existing menu permission.")
        .Produces<ApiResponse<MenuPermissionDto>>(
            StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        
        group.MapDelete("/", async (
            Guid menuId,
            Guid permissionId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new DeleteMenuPermissionCommand(menuId,permissionId),cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("DeleteMenuPermission")
        .WithSummary("Delete Menu Permission")
        .WithDescription("Deletes an existing menu permission.")
        .Produces<ApiResponse<bool>>(
            StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        
        group.MapGet("/{menuPermissionId:guid}", async (
            Guid menuPermissionId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetByIdMenuPermissionQuery(menuPermissionId),
                cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetByIdMenuPermission")
        .WithSummary("Get By Id Menu Permission")
        .WithDescription(
            "Returns a menu permission by its identifier.")
        .Produces<ApiResponse<MenuPermissionDto>>(
            StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        
        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetAllMenuPermissionQuery(request),cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetAllMenuPermissions")
        .WithSummary("Get All Menu Permissions")
        .WithDescription(
            "Returns a paginated list of menu permissions.")
        .Produces<ApiResponse<PaginatedResponse<MenuPermissionDto>>>(
            StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}