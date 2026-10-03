using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using BookMyHall.Domain.Constants;
using MediatR;

namespace BookMyHall.Api.Endpoints.Venue;

public static class VendorImageEndpoints
{
    public static void MapVendorImageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/vendors")
            .WithTags("Vendor Images")
            .DisableAntiforgery()
            .RequireAuthorization(policy => policy.RequireRole(RoleConstants.Admin, RoleConstants.HallOwner, RoleConstants.Vendor));

        group.MapPost("/{vendorId:guid}/images", async (
            Guid vendorId,
            IFormFile image,
            Guid? vendorServiceId,
            int displayOrder,
            bool isCoverImage,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            await using var stream = image.OpenReadStream();
            var command = new CreateVendorImageCommand(
                vendorId,
                stream,
                image.FileName,
                image.ContentType,
                image.Length,
                displayOrder,
                isCoverImage,
                vendorServiceId);

            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreateVendorImage")
        .WithSummary("Upload Vendor Image")
        .WithDescription("Uploads a vendor image. vendorServiceId is required and identifies the image's category and subcategory. JPG, PNG, or WEBP up to 5 MB.")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/images/{vendorImageId:guid}", async (
            Guid vendorImageId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorImageByIdQuery(vendorImageId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetVendorImageById")
        .WithSummary("Get Vendor Image")
        .Produces<ApiResponse<VendorImageDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{vendorId:guid}/images", async (
            Guid vendorId,
            Guid? vendorServiceId,
            Guid? vendorCategoryId,
            Guid? vendorSubCategoryId,
            [AsParameters] PaginationRequest pagination,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetVendorImagesByVendorIdQuery(vendorId, pagination, vendorServiceId, vendorCategoryId, vendorSubCategoryId),
                cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetVendorImagesByVendorId")
        .WithSummary("Get Vendor Images")
        .WithDescription("Returns active vendor images, optionally filtered by vendorServiceId, vendorCategoryId, or vendorSubCategoryId.")
        .Produces<ApiResponse<PaginatedResult<VendorImageDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{vendorId:guid}/cover-image", async (
            Guid vendorId,
            Guid? vendorServiceId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorCoverImageQuery(vendorId, vendorServiceId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetVendorCoverImage")
        .WithSummary("Get Vendor Cover Image")
        .WithDescription("Returns the vendor-level cover, or the service cover when vendorServiceId is supplied.")
        .Produces<ApiResponse<VendorImageDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/images/{vendorImageId:guid}", async (
            Guid vendorImageId,
            Guid? vendorServiceId,
            bool isCoverImage,
            int displayOrder,
            bool isActive,
            IFormFile? image,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            Stream? stream = null;
            try
            {
                string? fileName = null;
                string? contentType = null;
                long? fileSize = null;
                if (image is { Length: > 0 })
                {
                    stream = image.OpenReadStream();
                    fileName = image.FileName;
                    contentType = image.ContentType;
                    fileSize = image.Length;
                }

                var response = await mediator.Send(
                    new UpdateVendorImageCommand(
                        vendorImageId,
                        isCoverImage,
                        displayOrder,
                        isActive,
                        stream,
                        fileName,
                        contentType,
                        fileSize,
                        vendorServiceId),
                    cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            }
            finally
            {
                if (stream is not null)
                {
                    await stream.DisposeAsync();
                }
            }
        })
        .WithName("UpdateVendorImage")
        .WithSummary("Update Vendor Image")
        .WithDescription("Updates image metadata and optionally replaces the image. Supply vendorServiceId to assign a service; omitting it preserves the current assignment.")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<ApiResponse<VendorImageDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/images/{vendorImageId:guid}", async (
            Guid vendorImageId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new DeleteVendorImageCommand(vendorImageId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteVendorImage")
        .WithSummary("Delete Vendor Image")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/images/{vendorImageId:guid}/content", async (
            Guid vendorImageId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetVendorImageContentQuery(vendorImageId), cancellationToken);
            return result is null
                ? Results.NotFound()
                : Results.Stream(result.Stream, result.ContentType);
        })
        .WithName("GetVendorImageContent")
        .WithSummary("Get Vendor Image Content")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);
    }
}
