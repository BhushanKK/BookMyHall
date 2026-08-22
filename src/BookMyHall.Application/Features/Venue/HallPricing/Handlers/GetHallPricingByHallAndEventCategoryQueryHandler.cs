using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallPricingByHallAndEventCategoryQueryHandler(IHallPricingRepository hallPricingRepository,
    IMapper mapper,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetHallPricingByHallAndEventCategoryQuery, ApiResponse<HallPricingDto>>
{
    public async Task<ApiResponse<HallPricingDto>> Handle(GetHallPricingByHallAndEventCategoryQuery request,
        CancellationToken cancellationToken)
    {
         var cacheKey =
            $"{CacheKeys.HallPricing}:" +
            $"hall:{request.HallId}:" +
            $"event-category:{request.EventCategoryId}";

        var cachedHallPricing = await cacheService.GetAsync<HallPricingDto>(cacheKey, cancellationToken);

        if (cachedHallPricing is not null)
        {
            return ApiResponse<HallPricingDto>.SuccessResponse
            (
                cachedHallPricing,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallPricing),
                HttpStatusCode.OK
            );
        }

        var hallPricing = await hallPricingRepository.GetByHallIdAndEventCategoryIdAsync
        (
            request.HallId,
            request.EventCategoryId,
            cancellationToken
        );

        if (hallPricing is null)
        {
            return ApiResponse<HallPricingDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.HallPricing),
                HttpStatusCode.NotFound
            );
        }
        
        var response = mapper.Map<HallPricingDto>(hallPricing);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<HallPricingDto>.SuccessResponse
        (
            mapper.Map<HallPricingDto>(hallPricing),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.HallPricing),
            HttpStatusCode.OK
        );
    }
}