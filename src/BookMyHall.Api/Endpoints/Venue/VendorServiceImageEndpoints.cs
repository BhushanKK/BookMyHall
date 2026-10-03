using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using MediatR;

namespace BookMyHall.Api.Endpoints.Venue;

public static class VendorServiceImageEndpoints
{
    public static void MapVendorServiceImageEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/{vendorServiceId:guid}/images", async (
            Guid vendorServiceId, IFormFile image, int displayOrder, bool isCoverImage,
            IMediator mediator, CancellationToken cancellationToken) =>
        {
            var service = await mediator.Send(new GetVendorServiceByIdQuery(vendorServiceId), cancellationToken);
            if (!service.Success || service.Data is null)
                return Results.Json(service, statusCode: service.StatusCode);

            await using var stream = image.OpenReadStream();
            var response = await mediator.Send(new CreateVendorImageCommand(
                service.Data.VendorId, stream, image.FileName, image.ContentType, image.Length,
                displayOrder, isCoverImage, vendorServiceId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .DisableAntiforgery()
        .Accepts<IFormFile>("multipart/form-data")
        .WithName("UploadVendorServiceImage")
        .WithSummary("Upload an image for this vendor service")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{vendorServiceId:guid}/images", async (
            Guid vendorServiceId, [AsParameters] PaginationRequest pagination,
            IMediator mediator, CancellationToken cancellationToken) =>
        {
            var service = await mediator.Send(new GetVendorServiceByIdQuery(vendorServiceId), cancellationToken);
            if (!service.Success || service.Data is null)
                return Results.Json(service, statusCode: service.StatusCode);

            var response = await mediator.Send(new GetVendorImagesByVendorIdQuery(
                service.Data.VendorId, pagination, vendorServiceId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetVendorServiceImages")
        .WithSummary("Get images for this vendor service")
        .Produces<ApiResponse<PaginatedResult<VendorImageDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{vendorServiceId:guid}/cover-image", async (
            Guid vendorServiceId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var service = await mediator.Send(new GetVendorServiceByIdQuery(vendorServiceId), cancellationToken);
            if (!service.Success || service.Data is null)
                return Results.Json(service, statusCode: service.StatusCode);

            var response = await mediator.Send(new GetVendorCoverImageQuery(service.Data.VendorId, vendorServiceId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetVendorServiceCoverImage")
        .WithSummary("Get the cover image for this vendor service")
        .Produces<ApiResponse<VendorImageDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
