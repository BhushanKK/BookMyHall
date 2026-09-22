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
public sealed class GetCancellationPoliciesQueryHandler(ICancellationPolicyRepository cancellationPolicyRepository,
    IMessageHelper messageHelper,IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetCancellationPoliciesQuery, ApiResponse<PaginatedResponse<CancellationPolicy>>>
{
    public async Task<ApiResponse<PaginatedResponse<CancellationPolicy>>>Handle(GetCancellationPoliciesQuery request, CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<CancellationPolicy>(
            CacheKeys.CancellationPoliciesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<CancellationPolicy>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<CancellationPolicy>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.CancellationPolicy),
                HttpStatusCode.OK
            );
        }
        var result = await cancellationPolicyRepository.GetAllAsync(request.paginationRequest, cancellationToken);
        var response = new PaginatedResponse<CancellationPolicy>
        {
            Items = mapper.Map<IReadOnlyList<CancellationPolicy>>(result.Items),
            TotalRecords = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<CancellationPolicy>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.CancellationPolicy), HttpStatusCode.OK);
    }
}