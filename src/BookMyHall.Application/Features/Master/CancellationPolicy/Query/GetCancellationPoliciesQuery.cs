using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCancellationPoliciesQuery
    : IRequest<ApiResponse<PaginatedResult<CancellationPolicyDto>>>
{
    public PaginationRequest PaginationRequest { get; set; } = new();
}