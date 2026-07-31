using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCancellationPoliciesQueryHandler(
    ICancellationPolicyRepository cancellationPolicyRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetCancellationPoliciesQuery, ApiResponse<PaginatedResult<CancellationPolicyDto>>>
{
    public async Task<ApiResponse<PaginatedResult<CancellationPolicyDto>>> Handle(
        GetCancellationPoliciesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await cancellationPolicyRepository.GetAllAsync(
            request.PaginationRequest,
            cancellationToken);

        var response = new PaginatedResult<CancellationPolicyDto>
        {
            Items = mapper.Map<List<CancellationPolicyDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<CancellationPolicyDto>>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.CancellationPolicy),
            HttpStatusCode.OK);
    }
}