using MediatR;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;

namespace BookMyHall.Api.Endpoints.Venue;
public static class VendorServiceEndpoints
{
    public static void MapVendorServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/vendor-services")
            .WithTags("Vendor Services")
            .RequireAuthorization(policy =>policy.RequireRole(RoleConstants.Admin,RoleConstants.HallOwner));

        group.MapPost("/", async (CreateVendorServiceCommand command,IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("CreateVendorService")
        .WithSummary("Create Vendor Service")
        .WithDescription("Creates a new service for a vendor.")
        .Produces<ApiResponse<VendorServiceDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{vendorServiceId:guid}", async (Guid vendorServiceId,
            UpdateVendorServiceCommand command,IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.VendorServiceId =vendorServiceId;
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("UpdateVendorService")
        .WithSummary("Update Vendor Service")
        .WithDescription("Updates an existing vendor service.")
        .Produces<ApiResponse<VendorServiceDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{vendorServiceId:guid}",async (Guid vendorServiceId,
                IMediator mediator,CancellationToken cancellationToken) =>
            {
                var command =new DeleteVendorServiceCommand(vendorServiceId);
                var response = await mediator.Send(command,cancellationToken);
                return Results.Json(response,statusCode: response.StatusCode);
            })
        .WithName("DeleteVendorService")
        .WithSummary("Delete Vendor Service")
        .WithDescription("Soft deletes a vendor service.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{vendorServiceId:guid}",async (Guid vendorServiceId,
                IMediator mediator,CancellationToken cancellationToken) =>
            {
                var query = new GetVendorServiceByIdQuery(vendorServiceId);
                var response = await mediator.Send(query,cancellationToken);
                return Results.Json(response,statusCode: response.StatusCode);
            })
        .WithName("GetVendorServiceById")
        .WithSummary("Get Vendor Service")
        .WithDescription("Gets a vendor service by ID.")
        .Produces<ApiResponse<VendorServiceDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async ([AsParameters] PaginationRequest pagination,Guid? vendorId,
            Guid? vendorSubCategoryId,IMediator mediator,CancellationToken cancellationToken) =>
        {
            var query = new GetVendorServicesQuery(pagination,vendorId,vendorSubCategoryId);
            var response = await mediator.Send(query,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetVendorServices")
        .WithSummary("Get Vendor Services")
        .WithDescription("Gets paginated vendor services.")
        .Produces<ApiResponse<PaginatedResult<VendorServiceDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

         group.MapGet("/vendorservice/autocomplete", async (string? searchTerm,
            IMediator mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorServicesAutoCompleteQuery(searchTerm), cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetVendorServiceAutoComplete")
        .WithSummary("Get Vendor Service AutoComplete")
        .WithDescription("Returns up to 20 active vendorservice matching the optional search term.")
        .Produces<ApiResponse<IReadOnlyList<AutoCompleteItem>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}