using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;
public sealed record GetNearbyHallsQuery(
    double Latitude,
    double Longitude,
    double RadiusKm,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<ApiResponse<PaginatedResult<NearbyHallView>>>;