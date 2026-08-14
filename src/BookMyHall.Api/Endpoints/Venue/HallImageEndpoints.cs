using MediatR;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;

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

                var response = await mediator.Send(command,cancellationToken);
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
    }
}