using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;
using BookMyHall.Domain.Dtos;

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

        group.MapPost("/", async ([FromForm] VendorFormRequest request, IMapper mapper, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<CreateVendorCommand>(request);
            await using var stream = request.Logo?.OpenReadStream();
            if (request.Logo is not null)
            {
                command.Logo = new VendorLogoUpload(stream!, request.Logo.FileName, request.Logo.ContentType, request.Logo.Length);
            }
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .DisableAntiforgery()
        .Accepts<VendorFormRequest>("multipart/form-data")
        .WithName("CreateVendor")
        .WithSummary("Create Vendor")
        .WithDescription("Creates a vendor from form fields with an optional logo file (JPG, PNG, or WEBP, up to 5 MB).")
        .Produces<ApiResponse<VendorDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        // ---------------------------------------------------------
        // Update Vendor
        // ---------------------------------------------------------

        group.MapPut("/{vendorId:guid}", async (Guid vendorId, [FromForm] VendorFormRequest request,
            IMapper mapper, IMediator mediator,CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<UpdateVendorCommand>(request);
            command.VendorId = vendorId;
            await using var stream = request.Logo?.OpenReadStream();
            if (request.Logo is not null)
            {
                command.Logo = new VendorLogoUpload(stream!, request.Logo.FileName, request.Logo.ContentType, request.Logo.Length);
            }
            var response = await mediator.Send(command,cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .DisableAntiforgery()
        .Accepts<VendorFormRequest>("multipart/form-data")
        .WithName("UpdateVendor")
        .WithSummary("Update Vendor")
        .WithDescription("Updates vendor form fields and optionally replaces the logo. Omitting the logo preserves the existing file.")
        .Produces<ApiResponse<VendorDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        // ---------------------------------------------------------
        // Delete Vendor
        // ---------------------------------------------------------

        group.MapDelete("/{vendorId:guid}", async (Guid vendorId,IMediator mediator,CancellationToken cancellationToken) =>
        {
            var command = new DeleteVendorCommand(vendorId);
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("DeleteVendor")
        .WithSummary("Delete Vendor")
        .WithDescription("Deletes an existing vendor.")
        .Produces<ApiResponse<bool>>( StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

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
        .WithDescription("Returns a vendor by its identifier.")
        .Produces<ApiResponse<VendorDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // ---------------------------------------------------------
        // Get Vendors
        // ---------------------------------------------------------

        group.MapGet("/", async ([AsParameters] PaginationRequest request,
            IMediator mediator,CancellationToken cancellationToken, Guid? areaId = null) =>
        {
            var response = await mediator.Send(new GetVendorsQuery(request, areaId),cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetVendors")
        .WithSummary("Get Vendors")
        .WithDescription("Returns a paginated list of vendors.")
        .Produces<ApiResponse<PaginatedResponse<VendorListView>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
        
        // ---------------------------------------------------------
        //  Vendors Autocomplete
        // ---------------------------------------------------------
        group.MapGet("/owners/autocomplete", async (string? searchTerm,
            IMediator mediator, CancellationToken cancellationToken, int limit = 20) =>
        {
            var response = await mediator.Send(new GetVendorOwnersAutoCompleteQuery(searchTerm, limit), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetVendorOwnersAutoComplete")
        .WithSummary("Get active Vendor-role users for business ownership")
        .Produces<ApiResponse<IReadOnlyList<AutoCompleteItem>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/vendor/autocomplete", async (string? searchTerm,
            IMediator mediator, CancellationToken cancellationToken, Guid? areaId = null, int limit = 20) =>
        {
            var response = await mediator.Send(new GetVendorAutoCompleteQuery(searchTerm, areaId, limit), cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetVendorAutoComplete")
        .WithSummary("Get Vendors AutoComplete")
        .WithDescription("Returns up to 20 active vendors matching the optional search term.")
        .Produces<ApiResponse<IReadOnlyList<AutoCompleteItem>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/vendor/{vendorId:guid}/businesses/autocomplete", async (
            Guid vendorId,
            string? searchTerm,
            IMediator mediator,
            CancellationToken cancellationToken,
            Guid? areaId = null,
            int limit = 20) =>
        {
            var response = await mediator.Send(
                new GetVendorBusinessesAutoCompleteQuery(vendorId, searchTerm, areaId, limit),
                cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetVendorBusinessesAutoComplete")
        .WithSummary("Get Vendor Businesses AutoComplete")
        .WithDescription("Returns active businesses owned by the selected vendor.")
        .Produces<ApiResponse<IReadOnlyList<AutoCompleteItem>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);
    }
}
