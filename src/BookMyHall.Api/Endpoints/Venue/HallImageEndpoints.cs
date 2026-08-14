using MediatR;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;

namespace BookMyHall.Api.Endpoints.Venue;

public static class HallImageEndpoints
{
    public static void MapHallImageEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/halls")
            .WithTags("Hall Images")
            .DisableAntiforgery()
            .RequireAuthorization();

        group.MapPost("/{hallId:guid}/images",
            async (
                Guid hallId,
                IFormFile image,
                int displayOrder,
                bool isCoverImage,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                await using var stream = image.OpenReadStream();
                var command = new CreateHallImageCommand(
                    hallId,
                    stream,
                    image.FileName,
                    image.ContentType,
                    image.Length,
                    displayOrder,
                    isCoverImage);

                var response = await mediator.Send(command, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
        .WithName("CreateHallImage")
        .WithSummary("Upload Hall Image")
        .WithDescription("Uploads an image for a hall and stores it in Cloudflare R2.")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/images/{hallImageId:guid}",
            async (
                Guid hallImageId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetHallImageByIdQuery(hallImageId);
                var response = await mediator.Send(query,cancellationToken);
                return Results.Json(response,statusCode: response.StatusCode);
            })
        .WithName("GetHallImageById")
        .WithSummary("Get Hall Image")
        .WithDescription("Returns a hall image by its identifier.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{hallId:guid}/images",
            async (
                Guid hallId,
                [AsParameters] PaginationRequest pagination,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetHallImagesByHallIdQuery(hallId, pagination);
                var response = await mediator.Send(query, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
        .WithName("GetHallImagesByHallId")
        .WithSummary("Get Hall Images")
        .WithDescription("Returns paginated active images belonging to a hall.")
        .Produces<ApiResponse<PaginatedResult<HallImageDto>>>(
            StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{hallId:guid}/cover-image",
            async (
                Guid hallId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetHallCoverImageQuery(hallId);
                var response = await mediator.Send(query,cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
        .WithName("GetHallCoverImage")
        .WithSummary("Get Hall Cover Image")
        .WithDescription("Returns the active cover image for a hall.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/images/{hallImageId:guid}",
            async (
                Guid hallImageId,
                UpdateHallImageCommand request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateHallImageCommand(
                    hallImageId,
                    request.IsCoverImage,
                    request.DisplayOrder,
                    request.IsActive);

                var response = await mediator.Send(command, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
        .WithName("UpdateHallImage")
        .WithSummary("Update Hall Image")
        .WithDescription("Updates hall image metadata such as cover image, display order and active status.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/images/{hallImageId:guid}",
            async (
                Guid hallImageId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteHallImageCommand(hallImageId);
                var response = await mediator.Send(command, cancellationToken);
                return Results.Json(response,statusCode: response.StatusCode);
            })
        .WithName("DeleteHallImage")
        .WithSummary("Delete Hall Image")
        .WithDescription("Soft deletes a hall image.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/images/{hallImageId:guid}/content",
            async (
                Guid hallImageId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetHallImageContentQuery(hallImageId);
                var result = await mediator.Send(query, cancellationToken);

                if (result is null)
                    return Results.NotFound();

                return Results.Stream(result.Stream, result.ContentType);
            })
        .WithName("GetHallImageContent")
        .WithSummary("Get Hall Image Content")
        .WithDescription(
            "Returns the hall image from private Cloudflare R2 storage.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);
    }
}