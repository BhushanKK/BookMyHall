using System.Net;

using AutoMapper;
using MediatR;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallPricingByHallAndEventCategoryQueryHandler(
    IHallPricingRepository hallPricingRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        GetHallPricingByHallAndEventCategoryQuery,
        ApiResponse<HallPricingDto>>
{
    public async Task<ApiResponse<HallPricingDto>> Handle(
        GetHallPricingByHallAndEventCategoryQuery request,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // BUILD CACHE KEY
        // =========================================================

        var cacheKey =
            HallPricingCacheKeyBuilder
                .BuildByHallAndEventCategoryKey(
                    request.HallId,
                    request.EventCategoryId);

        // =========================================================
        // CHECK CACHE
        // =========================================================

        var cachedHallPricing =
            await cacheService.GetAsync<HallPricingDto>(
                cacheKey,
                cancellationToken);

        if (cachedHallPricing is not null)
        {
            return ApiResponse<HallPricingDto>
                .SuccessResponse(
                    cachedHallPricing,
                    messageHelper.RetrievedEntity(
                        ResourceNames.Entities,
                        EntityKeys.HallPricing),
                    HttpStatusCode.OK);
        }

        // =========================================================
        // GET FROM DATABASE
        // =========================================================

        var hallPricing =
            await hallPricingRepository
                .GetByHallIdAndEventCategoryIdAsync(
                    request.HallId,
                    request.EventCategoryId,
                    cancellationToken);

        if (hallPricing is null)
        {
            return ApiResponse<HallPricingDto>
                .FailureResponse(
                    messageHelper.NotFoundEntity(
                        ResourceNames.Entities,
                        EntityKeys.HallPricing),
                    HttpStatusCode.NotFound);
        }

        // =========================================================
        // MAP ENTITY -> DTO
        // =========================================================

        var response =
            mapper.Map<HallPricingDto>(
                hallPricing);

        // =========================================================
        // STORE IN CACHE
        // =========================================================

        await cacheService.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(30),
            cancellationToken);

        // =========================================================
        // RETURN SUCCESS
        // =========================================================

        return ApiResponse<HallPricingDto>
            .SuccessResponse(
                response,
                messageHelper.RetrievedEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallPricing),
                HttpStatusCode.OK);
    }
}