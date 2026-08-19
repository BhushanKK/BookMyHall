using MediatR;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Api.Endpoints.Identity;

public static class PermissionEndpoints
{
    public static void MapPermissionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/permissions")
            .WithTags("Permissions")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (CreatePermissionCommand command,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command,cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("CreatePermission")
        .WithSummary("Create Permission")
        .WithDescription("Creates a new permission.")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{permissionId:guid}", async (Guid permissionId,
            UpdatePermissionCommand command,IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.PermissionId = permissionId;
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("UpdatePermission")
        .WithSummary("Update Permission")
        .WithDescription("Updates an existing permission.")
        .Produces<ApiResponse<PermissionDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{permissionId:guid}", async (Guid permissionId,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new DeletePermissionCommand(permissionId),cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("DeletePermission")
        .WithSummary("Delete Permission")
        .WithDescription("Deletes an existing permission.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{permissionId:guid}", async (Guid permissionId,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetPermissionByIdQuery(permissionId),cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetPermissionById")
        .WithSummary("Get Permission By Id")
        .WithDescription("Returns a permission by its identifier.")
        .Produces<ApiResponse<PermissionDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async ([AsParameters] PaginationRequest request,IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetPermissionQuery(request),cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetPermissions")
        .WithSummary("Get Permissions")
        .WithDescription("Returns a paginated list of permissions.")
        .Produces<ApiResponse<PaginatedResponse<Permission>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}