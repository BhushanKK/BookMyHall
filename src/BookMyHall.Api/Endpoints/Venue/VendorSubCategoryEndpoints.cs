using MediatR;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;

namespace BookMyHall.Api.Endpoints.Venue;
public static class VendorSubCategoryEndpoints
{
    public static void MapVendorSubCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/vendor-sub-categories")
            .WithTags("Vendor Sub Categories")
            .RequireAuthorization(policy =>policy.RequireRole(RoleConstants.Admin,RoleConstants.HallOwner));

        group.MapPost("/", async (CreateVendorSubCategoryCommand command,IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command,cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("CreateVendorSubCategory")
        .WithSummary("Create Vendor Sub Category")
        .WithDescription("Creates a new vendor sub category.")
        .Produces<ApiResponse<VendorSubCategoryDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{vendorSubCategoryId:guid}",async (Guid vendorSubCategoryId,
                UpdateVendorSubCategoryCommand command,
                IMediator mediator,CancellationToken cancellationToken) =>
            {
                command.VendorSubCategoryId =vendorSubCategoryId;
                var response = await mediator.Send(command,cancellationToken);
                return Results.Json(response,statusCode: response.StatusCode);
            })
        .WithName("UpdateVendorSubCategory")
        .WithSummary("Update Vendor Sub Category")
        .WithDescription("Updates an existing vendor sub category.")
        .Produces<ApiResponse<VendorSubCategoryDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{vendorSubCategoryId:guid}",async (Guid vendorSubCategoryId,
                IMediator mediator,CancellationToken cancellationToken) =>
            {
                var command =new DeleteVendorSubCategoryCommand(vendorSubCategoryId);
                var response = await mediator.Send(command,cancellationToken);
                return Results.Json(response,statusCode: response.StatusCode);
            })
        .WithName("DeleteVendorSubCategory")
        .WithSummary("Delete Vendor Sub Category")
        .WithDescription("Soft deletes a vendor sub category.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{vendorSubCategoryId:guid}",async (Guid vendorSubCategoryId,
                IMediator mediator,CancellationToken cancellationToken) =>
            {
                var query =new GetVendorSubCategoryByIdQuery(vendorSubCategoryId);
                var response = await mediator.Send(query,cancellationToken);
                return Results.Json(response,statusCode: response.StatusCode);
            })
        .WithName("GetVendorSubCategoryById")
        .WithSummary("Get Vendor Sub Category")
        .WithDescription("Gets a vendor sub category by ID.")
        .Produces<ApiResponse<VendorSubCategoryDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async ([AsParameters] PaginationRequest pagination,Guid? vendorCategoryId,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var query =new GetVendorSubCategoriesQuery(pagination,vendorCategoryId);
            var response = await mediator.Send(query,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetVendorSubCategories")
        .WithSummary("Get Vendor Sub Categories")
        .WithDescription("Gets paginated vendor sub categories.")
        .Produces<ApiResponse<PaginatedResult<VendorSubCategoryDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}