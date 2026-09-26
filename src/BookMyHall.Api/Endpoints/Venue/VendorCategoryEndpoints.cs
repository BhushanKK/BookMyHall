using MediatR;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;
namespace BookMyHall.Api.Endpoints.Venue;
public static class VendorCategoryEndpoints
{
    public static void MapVendorCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/vendor-categories")
            .WithTags("Vendor Category")
            .RequireAuthorization(policy =>policy.RequireRole(
                    RoleConstants.Admin,
                    RoleConstants.HallOwner));

        group.MapPost("/", async (CreateVendorCategoryCommand command,IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("CreateVendorCategory")
        .WithSummary("Create Vendor Category")
        .WithDescription("Creates a new vendor category.")
        .Produces<ApiResponse<VendorCategoryDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{vendorCategoryId:guid}", async (Guid vendorCategoryId,UpdateVendorCategoryCommand command,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            command.VendorCategoryId =vendorCategoryId;
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("UpdateVendorCategory")
        .WithSummary("Update Vendor Category")
        .WithDescription("Updates an existing vendor category.")
        .Produces<ApiResponse<VendorCategoryDto>>(StatusCodes.Status200OK)
        .Produces( StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{vendorCategoryId:guid}", async (Guid vendorCategoryId,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var command =new DeleteVendorCategoryCommand(vendorCategoryId);
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("DeleteVendorCategory")
        .WithSummary("Delete Vendor Category")
        .WithDescription("Deletes an existing vendor category.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{vendorCategoryId:guid}", async (Guid vendorCategoryId,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorCategoryByIdQuery(vendorCategoryId),cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetVendorCategoryById")
        .WithSummary("Get Vendor Category By Id")
        .WithDescription("Returns a vendor category by its identifier.")
        .Produces<ApiResponse<VendorCategoryDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async ([AsParameters] PaginationRequest request,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorCategoriesQuery(request),cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetVendorCategories")
        .WithSummary("Get Vendor Categories")
        .WithDescription("Returns a paginated list of vendor categories.")
        .Produces<ApiResponse<PaginatedResult<VendorCategoryDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}