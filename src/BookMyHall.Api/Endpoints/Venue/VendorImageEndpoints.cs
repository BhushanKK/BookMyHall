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
            .RequireAuthorization(policy => policy.RequireRole(RoleConstants.Admin, RoleConstants.Vendor));

        group.MapPost("/{vendorId:guid}/images", async (
            Guid vendorId,
            IFormFile image,
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
                isCoverImage);

            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreateVendorImage")
        .WithSummary("Upload Vendor Image")
        .WithDescription("Uploads an image for a vendor and stores it in Cloudflare R2.")
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
            [AsParameters] PaginationRequest pagination,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetVendorImagesByVendorIdQuery(vendorId, pagination),
                cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetVendorImagesByVendorId")
        .WithSummary("Get Vendor Images")
        .Produces<ApiResponse<PaginatedResult<VendorImageDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{vendorId:guid}/cover-image", async (
            Guid vendorId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorCoverImageQuery(vendorId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetVendorCoverImage")
        .WithSummary("Get Vendor Cover Image")
        .Produces<ApiResponse<VendorImageDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/images/{vendorImageId:guid}", async (
            Guid vendorImageId,
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
                        fileSize),
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
        .WithDescription("Updates vendor image metadata and optionally replaces the image.")
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