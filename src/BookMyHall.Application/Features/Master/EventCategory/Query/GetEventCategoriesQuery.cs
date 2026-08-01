using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetEventCategoriesQuery(PaginationRequest PaginationRequest)
    : IRequest<ApiResponse<PaginatedResult<EventCategoryDto>>>
{
}