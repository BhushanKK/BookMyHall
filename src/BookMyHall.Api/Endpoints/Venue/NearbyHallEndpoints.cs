using MediatR;

using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Api.Endpoints.Venue;

public static class NearbyHallEndpoints
{
    public static void MapNearbyHallEndpoints(
     this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/halls/nearby")
            .WithTags("Nearby Hall");

        group.MapGet(
            "/",
            async (
                double latitude,
                double longitude,
                double radiusKm,
                Guid? stateId,
                Guid? districtId,
                Guid? cityId,
                Guid? areaId,
                int pageNumber,
                int pageSize,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetNearbyHallsQuery(
                    Latitude: latitude,
                    Longitude: longitude,
                    RadiusKm: radiusKm,
                    StateId: stateId,
                    DistrictId: districtId,
                    CityId: cityId,
                    AreaId: areaId,
                    PageNumber: pageNumber,
                    PageSize: pageSize);

                var response = await mediator.Send(
                    query,
                    cancellationToken);

                return Results.Json(
                    response,
                    statusCode: response.StatusCode);
            })
            .AllowAnonymous()
            .WithName("GetNearbyHalls")
            .WithSummary("Get Nearby Halls")
            .WithDescription(
                "Returns a paginated list of active halls near " +
                "the specified latitude and longitude, optionally " +
                "filtered by state, district, city, and area.")
            .Produces<ApiResponse<PaginatedResult<NearbyHallView>>>(
                StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}