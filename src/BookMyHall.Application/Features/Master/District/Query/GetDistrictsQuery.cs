using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetDistrictsQuery (PaginationRequest Request)
    :IRequest<ApiResponse<PaginatedResult<DistrictDto>>>
{
}