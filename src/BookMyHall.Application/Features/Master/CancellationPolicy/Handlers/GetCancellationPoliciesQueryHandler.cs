using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCancellationPoliciesQueryHandler(
    ICancellationPolicyRepository cancellationPolicyRepository,
    IMessageHelper messageHelper,
    IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetCancellationPoliciesQuery, ApiResponse<PaginatedResult<CancellationPolicy>>>
{
    public async Task<ApiResponse<PaginatedResult<CancellationPolicy>>> Handle(GetCancellationPoliciesQuery request, CancellationToken cancellationToken)
    {

        var pagination = request.paginationRequest;

        var cacheKey =
            $"{CacheKeys.CancellationPolicy}:" +
            $"page:{pagination.PageNumber}:" +
            $"size:{pagination.PageSize}";

        var cachedResponse = await cacheService.GetAsync<PaginatedResult<CancellationPolicy>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResult<CancellationPolicy>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.CancellationPolicy),
                HttpStatusCode.OK
            );
        }
        var result = await cancellationPolicyRepository.GetAllAsync(request.paginationRequest, cancellationToken);
        var response = new PaginatedResult<CancellationPolicy>
        {
            Items = mapper.Map<IReadOnlyList<CancellationPolicy>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<PaginatedResult<CancellationPolicy>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.CancellationPolicy), HttpStatusCode.OK);
    }
}