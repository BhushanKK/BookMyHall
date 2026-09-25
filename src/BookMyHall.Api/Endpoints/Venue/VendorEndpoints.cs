using MediatR;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;

namespace BookMyHall.Api.Endpoints.Venue;

public static class VendorEndpoints
{
    public static void MapVendorEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/vendors")
            .WithTags("Vendor")
            .RequireAuthorization(policy =>
                policy.RequireRole(
                    RoleConstants.Admin,
                    RoleConstants.HallOwner));

        // ---------------------------------------------------------
        // Create Vendor
        // ---------------------------------------------------------

        group.MapPost("/", async (
            CreateVendorCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                command,
                cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("CreateVendor")
        .WithSummary("Create Vendor")
        .WithDescription("Creates a new vendor.")
        .Produces<ApiResponse<VendorDto>>(
            StatusCodes.Status201Created)
        .Produces(
            StatusCodes.Status400BadRequest)
        .Produces(
            StatusCodes.Status401Unauthorized)
        .Produces(
            StatusCodes.Status409Conflict);

        // ---------------------------------------------------------
        // Update Vendor
        // ---------------------------------------------------------

        group.MapPut("/{vendorId:guid}", async (
            Guid vendorId,
            UpdateVendorCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.VendorId = vendorId;

            var response = await mediator.Send(
                command,
                cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("UpdateVendor")
        .WithSummary("Update Vendor")
        .WithDescription("Updates an existing vendor.")
        .Produces<ApiResponse<VendorDto>>(
            StatusCodes.Status200OK)
        .Produces(
            StatusCodes.Status400BadRequest)
        .Produces(
            StatusCodes.Status401Unauthorized)
        .Produces(
            StatusCodes.Status404NotFound)
        .Produces(
            StatusCodes.Status409Conflict);

        // ---------------------------------------------------------
        // Delete Vendor
        // ---------------------------------------------------------

        group.MapDelete("/{vendorId:guid}", async (
            Guid vendorId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteVendorCommand(vendorId);

            var response = await mediator.Send(
                command,
                cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("DeleteVendor")
        .WithSummary("Delete Vendor")
        .WithDescription("Deletes an existing vendor.")
        .Produces<ApiResponse<bool>>(
            StatusCodes.Status200OK)
        .Produces(
            StatusCodes.Status401Unauthorized)
        .Produces(
            StatusCodes.Status404NotFound);

        // ---------------------------------------------------------
        // Get Vendor By Id
        // ---------------------------------------------------------

        group.MapGet("/{vendorId:guid}", async (Guid vendorId,IMediator mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorByIdQuery(vendorId),cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetVendorById")
        .WithSummary("Get Vendor By Id")
        .WithDescription(
            "Returns a vendor by its identifier.")
        .Produces<ApiResponse<VendorDto>>(
            StatusCodes.Status200OK)
        .Produces(
            StatusCodes.Status401Unauthorized)
        .Produces(
            StatusCodes.Status404NotFound);

        // ---------------------------------------------------------
        // Get Vendors
        // ---------------------------------------------------------

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetVendorsQuery(request),
                cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetVendors")
        .WithSummary("Get Vendors")
        .WithDescription("Returns a paginated list of vendors.")
        .Produces<ApiResponse<PaginatedResult<VendorDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}