using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCancellationPoliciesQueryHandler(
    ICancellationPolicyRepository cancellationPolicyRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetCancellationPoliciesQuery, ApiResponse<PaginatedResult<CancellationPolicy>>>
{
    public async Task<ApiResponse<PaginatedResult<CancellationPolicy>>> Handle(GetCancellationPoliciesQuery request,CancellationToken cancellationToken)
    {
        var result = await cancellationPolicyRepository.GetAllAsync(request.paginationRequest,cancellationToken);
        var response = new PaginatedResult<CancellationPolicy>
        {
            Items = mapper.Map<IReadOnlyList<CancellationPolicy>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<CancellationPolicy>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.CancellationPolicy),HttpStatusCode.OK);
    }
}