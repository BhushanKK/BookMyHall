using MediatR;

using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;

namespace BookMyHall.Api.Endpoints.Venue;

public static class VendorPackageItemEndpoints
{
    public static void MapVendorPackageItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/vendor-package-items")
            .WithTags("Vendor Package Items")
            .RequireAuthorization(policy => policy.RequireRole(RoleConstants.Admin, RoleConstants.HallOwner));

        group.MapPost("/", async (CreateVendorPackageItemCommand command,
                    IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var response = await mediator.Send(command, cancellationToken);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
            .WithName("CreateVendorPackageItem")
            .WithSummary("Create Vendor Package Item")
            .Produces<ApiResponse<VendorPackageItemDto>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{vendorPackageItemId:guid}", async (Guid vendorPackageItemId,
                    UpdateVendorPackageItemCommand command, IMediator mediator,
                    CancellationToken cancellationToken) =>
                {
                    command.VendorPackageItemId = vendorPackageItemId;
                    var response = await mediator.Send(command, cancellationToken);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
            .WithName("UpdateVendorPackageItem")
            .WithSummary("Update Vendor Package Item")
            .Produces<ApiResponse<VendorPackageItemDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{vendorPackageItemId:guid}", async (Guid vendorPackageItemId,
                    IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteVendorPackageItemCommand(vendorPackageItemId);
                    var response = await mediator.Send(command, cancellationToken);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
            .WithName("DeleteVendorPackageItem")
            .WithSummary("Delete Vendor Package Item")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{vendorPackageItemId:guid}", async (Guid vendorPackageItemId,
                    IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var query = new GetVendorPackageItemByIdQuery(vendorPackageItemId);
                    var response = await mediator.Send(query, cancellationToken);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
            .WithName("GetVendorPackageItemById")
            .WithSummary("Get Vendor Package Item")
            .Produces<ApiResponse<VendorPackageItemDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async ([AsParameters] PaginationRequest pagination,
                    Guid? vendorPackageId, IMediator mediator,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetVendorPackageItemsQuery(pagination, vendorPackageId);
                    var response = await mediator.Send(query, cancellationToken);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
            .WithName("GetVendorPackageItems")
            .WithSummary("Get Vendor Package Items")
            .Produces<ApiResponse<PaginatedResult<VendorPackageItemDto>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}