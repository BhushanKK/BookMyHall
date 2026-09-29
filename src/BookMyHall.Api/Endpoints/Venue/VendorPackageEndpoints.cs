using MediatR;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Api.Endpoints.Venue;

public static class VendorPackageEndpoints
{
    public static void MapVendorPackageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/vendor-packages")
            .WithTags("Vendor Packages")
           .RequireAuthorization(policy => policy.RequireRole
        (
            RoleConstants.Admin, 
            RoleConstants.Vendor
        ));

        group.MapPost("/", async (CreateVendorPackageCommand command,
            IMediator mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreateVendorPackage")
        .WithSummary("Create Vendor Package")
        .Produces<ApiResponse<VendorPackageDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{packageId:guid}", async (
            Guid packageId,
            UpdateVendorPackageCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.VendorPackageId = packageId;
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdateVendorPackage")
        .WithSummary("Update Vendor Package")
        .Produces<ApiResponse<VendorPackageDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{packageId:guid}", async (
            Guid packageId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new DeleteVendorPackageCommand(packageId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteVendorPackage")
        .WithSummary("Delete Vendor Package")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{packageId:guid}", async (Guid packageId,
            IMediator mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorPackageByIdQuery(packageId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetVendorPackageById")
        .WithSummary("Get Vendor Package By Id")
        .Produces<ApiResponse<VendorPackage>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async ([AsParameters] PaginationRequest request,
            IMediator mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorPackagesQuery(request), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetVendorPackages")
        .WithSummary("Get Vendor Packages List")
        .Produces<ApiResponse<PaginatedResponse<VendorPackage>>>(StatusCodes.Status200OK);

         group.MapGet("/vendorpackage/autocomplete", async (string? searchTerm,
            IMediator mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorPackagesAutoCompleteQuery(searchTerm), cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetVendorPackageAutoComplete")
        .WithSummary("Get Vendor Package AutoComplete")
        .WithDescription("Returns up to 20 active vendorpackage matching the optional search term.")
        .Produces<ApiResponse<IReadOnlyList<AutoCompleteItem>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
