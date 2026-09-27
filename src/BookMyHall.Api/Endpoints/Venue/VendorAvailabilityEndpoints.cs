using MediatR;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;
namespace BookMyHall.Api.Endpoints.Venue;

public static class VendorAvailabilityEndpoints
{
    public static void MapVendorAvailabilityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/vendor-availabilities")
            .WithTags("Vendor Availability")
            .RequireAuthorization(policy =>policy.RequireRole(RoleConstants.Admin,RoleConstants.HallOwner));

        group.MapPost("/", async (CreateVendorAvailabilityCommand command,IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("CreateVendorAvailability")
        .WithSummary("Create Vendor Availability")
        .WithDescription("Creates a new vendor availability.")
        .Produces<ApiResponse<VendorAvailabilityDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{vendorAvailabilityId:guid}", async (Guid vendorAvailabilityId,UpdateVendorAvailabilityCommand command,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            command.VendorAvailabilityId = vendorAvailabilityId;
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("UpdateVendorAvailability")
        .WithSummary("Update Vendor Availability")
        .WithDescription("Updates an existing vendor availability.")
        .Produces<ApiResponse<VendorAvailabilityDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{vendorAvailabilityId:guid}", async (Guid vendorAvailabilityId,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var command =new DeleteVendorAvailabilityCommand(vendorAvailabilityId);
            var response = await mediator.Send(command,cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteVendorAvailability")
        .WithSummary("Delete Vendor Availability")
        .WithDescription("Deletes an existing vendor availability.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{vendorAvailabilityId:guid}", async (Guid vendorAvailabilityId,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorAvailabilityByIdQuery(vendorAvailabilityId),cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetVendorAvailabilityById")
        .WithSummary("Get Vendor Availability By Id")
        .WithDescription("Returns a vendor availability by its identifier.")
        .Produces<ApiResponse<VendorAvailabilityDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async ([AsParameters] PaginationRequest request,Guid? vendorId,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorAvailabilitiesQuery(request,vendorId),cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetVendorAvailabilities")
        .WithSummary("Get Vendor Availabilities")
        .WithDescription("Returns a paginated list of vendor availabilities.")
        .Produces<ApiResponse<PaginatedResult<VendorAvailabilityDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}