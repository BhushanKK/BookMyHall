using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetHallImagesByHallIdQuery(Guid HallId, PaginationRequest Pagination)
    : IRequest<ApiResponse<PaginatedResult<HallImageDto>>>;