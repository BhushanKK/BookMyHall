using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;
public sealed record GetHallPricingQuery(PaginationRequest PaginationRequest)
    : IRequest<ApiResponse<PaginatedResult<HallPricingDto>>>;