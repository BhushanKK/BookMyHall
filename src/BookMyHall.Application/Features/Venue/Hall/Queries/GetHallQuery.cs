using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetHallQuery(PaginationRequest paginationRequest)
    : IRequest<ApiResponse<PaginatedResult<HallListView>>>;