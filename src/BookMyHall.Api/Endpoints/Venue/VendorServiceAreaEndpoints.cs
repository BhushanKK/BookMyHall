using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;

using MediatR;

namespace BookMyHall.Api.Endpoints.Venue;

public static class VendorServiceAreaEndpoints
{
    public static void MapVendorServiceAreaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/vendor-service-areas")
            .WithTags("Vendor Service Areas")
            .RequireAuthorization(policy => policy.RequireRole(RoleConstants.Admin, RoleConstants.HallOwner));

        group.MapPost("/", async (CreateVendorServiceAreaCommand command,
                    IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var response = await mediator.Send(command, cancellationToken);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
            .WithName("CreateVendorServiceArea")
            .WithSummary("Create Vendor Service Area")
            .Produces<ApiResponse<VendorServiceAreaDto>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{vendorServiceAreaId:guid}", async (Guid vendorServiceAreaId,
                    UpdateVendorServiceAreaCommand command, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    command.VendorServiceAreaId = vendorServiceAreaId;
                    var response = await mediator.Send(command, cancellationToken);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
            .WithName("UpdateVendorServiceArea")
            .WithSummary("Update Vendor Service Area")
            .Produces<ApiResponse<VendorServiceAreaDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{vendorServiceAreaId:guid}",
                async (Guid vendorServiceAreaId, IMediator mediator,
                    CancellationToken cancellationToken) =>
                {
                    var command = new DeleteVendorServiceAreaCommand(vendorServiceAreaId);
                    var response = await mediator.Send(command, cancellationToken);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
            .WithName("DeleteVendorServiceArea")
            .WithSummary("Delete Vendor Service Area")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{vendorServiceAreaId:guid}", async (Guid vendorServiceAreaId,
                    IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var query = new GetVendorServiceAreaByIdQuery(vendorServiceAreaId);
                    var response = await mediator.Send(query, cancellationToken);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
            .WithName("GetVendorServiceAreaById")
            .WithSummary("Get Vendor Service Area")
            .Produces<ApiResponse<VendorServiceAreaDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async ([AsParameters] PaginationRequest pagination,
                    Guid? vendorId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var query = new GetVendorServiceAreasQuery(pagination, vendorId);
                    var response = await mediator.Send(query, cancellationToken);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
            .WithName("GetVendorServiceAreas")
            .WithSummary("Get Vendor Service Areas")
            .Produces<ApiResponse<PaginatedResult<VendorServiceAreaDto>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}