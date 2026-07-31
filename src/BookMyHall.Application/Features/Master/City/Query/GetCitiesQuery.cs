using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetCitiesQuery (PaginationRequest Request)
    :IRequest<ApiResponse<PaginatedResult<CityDto>>>
{
}