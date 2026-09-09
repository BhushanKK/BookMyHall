using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetNearbyHallsQuery(
    double Latitude,
    double Longitude,
    double RadiusKm,

    Guid? StateId = null,
    Guid? DistrictId = null,
    Guid? CityId = null,
    Guid? AreaId = null,

    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<ApiResponse<PaginatedResult<NearbyHallView>>>;