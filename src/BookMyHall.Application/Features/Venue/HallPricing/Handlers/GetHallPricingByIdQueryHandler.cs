using System.Net;

using AutoMapper;
using MediatR;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallPricingByIdQueryHandler(
    IHallPricingRepository hallPricingRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        GetHallPricingByIdQuery,
        ApiResponse<HallPricingDto>>
{
    public async Task<ApiResponse<HallPricingDto>> Handle(
        GetHallPricingByIdQuery request,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // CACHE KEY
        // =========================================================

        var cacheKey =
            HallPricingCacheKeyBuilder
                .BuildByIdKey(
                    request.HallPricingId);

        // =========================================================
        // CACHE
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
        // DATABASE
        // =========================================================

        var hallPricing =
            await hallPricingRepository.GetByIdAsync(
                request.HallPricingId,
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
        // MAP
        // =========================================================

        var response =
            mapper.Map<HallPricingDto>(
                hallPricing);

        // =========================================================
        // CACHE
        // =========================================================

        await cacheService.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(30),
            cancellationToken);

        // =========================================================
        // RETURN
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