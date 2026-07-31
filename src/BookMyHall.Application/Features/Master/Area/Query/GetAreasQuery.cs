using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class GetAreasQuery
    : IRequest<ApiResponse<PaginatedResult<AreaDto>>>
{
    public PaginationRequest PaginationRequest { get; set; } = new();
}