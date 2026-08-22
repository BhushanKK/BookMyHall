using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetHallBlocksQuery(PaginationRequest paginationRequest)
: IRequest<ApiResponse<PaginatedResponse<HallBlockDto>>>;