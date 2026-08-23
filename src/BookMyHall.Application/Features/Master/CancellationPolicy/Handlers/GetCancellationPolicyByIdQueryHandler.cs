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

public sealed class GetCancellationPolicyByIdQueryHandler(
    ICancellationPolicyRepository cancellationPolicyRepository,
    IMessageHelper messageHelper,
    IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetCancellationPolicyByIdQuery, ApiResponse<CancellationPolicy>>
{
    public async Task<ApiResponse<CancellationPolicy>> Handle(GetCancellationPolicyByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.CancellationPolicies}:{request.CancellationPolicyId}";
        var cachedCancellationPolicy = await cacheService.GetAsync<CancellationPolicy>(cacheKey, cancellationToken);

        if (cachedCancellationPolicy is not null)
        {
            return ApiResponse<CancellationPolicy>.SuccessResponse(cachedCancellationPolicy, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.CancellationPolicy), HttpStatusCode.OK);
        }
        var policy = await cancellationPolicyRepository.GetByIdAsync(request.CancellationPolicyId, cancellationToken);
        if (policy is null)
        {
            return ApiResponse<CancellationPolicy>.FailureResponse(
                messageHelper.NotFound(EntityKeys.CancellationPolicy),
                HttpStatusCode.NotFound);
        }
        var response = mapper.Map<CancellationPolicy>(policy);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<CancellationPolicy>.SuccessResponse(
            mapper.Map<CancellationPolicy>(policy),
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.CancellationPolicy), HttpStatusCode.OK);
    }
}