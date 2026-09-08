using MediatR;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Api.Endpoints.Venue;

public static class NearbyHallEndpoints
{
    public static void MapNearbyHallEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/halls/nearby")
            .WithTags("Nearby Hall");

        group.MapGet("/", async (
            double latitude,
            double longitude,
            double radiusKm,
            int pageNumber,
            int pageSize,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetNearbyHallsQuery(
                latitude,
                longitude,
                radiusKm,
                pageNumber,
                pageSize);

            var response = await mediator.Send(query,cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .AllowAnonymous()
        .WithName("GetNearbyHalls")
        .WithSummary("Get Nearby Halls")
        .WithDescription("Returns a paginated list of active halls near the specified latitude and longitude.")
        .Produces<ApiResponse<PaginatedResult<NearbyHallView>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}