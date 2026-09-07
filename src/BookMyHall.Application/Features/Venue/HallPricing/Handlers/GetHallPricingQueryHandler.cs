using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallPricingQueryHandler(
    IHallPricingRepository hallPricingRepository,
    IMessageHelper messageHelper,
    IMapper mapper,
    ICacheService cacheService)
    : IRequestHandler<
        GetHallPricingQuery,
        ApiResponse<PaginatedResult<HallPricingDto>>>
{
    public async Task<
        ApiResponse<PaginatedResult<HallPricingDto>>>
        Handle(
            GetHallPricingQuery request,
            CancellationToken cancellationToken)
    {
        // =========================================================
        // PAGINATION
        // =========================================================

        var pagination =
            request.paginationRequest;

        // =========================================================
        // CACHE KEY
        // =========================================================

        var cacheKey =
            HallPricingCacheKeyBuilder
                .BuildPaginatedKey(
                    pagination.PageNumber,
                    pagination.PageSize,
                    pagination.SearchText,
                    pagination.SortBy,
                    pagination.SortDescending);

        // =========================================================
        // CACHE
        // =========================================================

        var cachedResponse =
            await cacheService.GetAsync<
                PaginatedResult<HallPricingDto>>(
                cacheKey,
                cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<
                PaginatedResult<HallPricingDto>>
                .SuccessResponse(
                    cachedResponse,
                    messageHelper.RetrievedEntity(
                        ResourceNames.Entities,
                        EntityKeys.HallPricing),
                    HttpStatusCode.OK);
        }

        // =========================================================
        // DATABASE
        // =========================================================

        var result =
            await hallPricingRepository.GetAllAsync(
                pagination,
                cancellationToken);

        // =========================================================
        // MAP
        // =========================================================

        var response =
            new PaginatedResult<HallPricingDto>
            {
                Items =
                    mapper.Map<
                        IReadOnlyList<HallPricingDto>>(
                        result.Items),

                TotalCount =
                    result.TotalCount,

                PageNumber =
                    result.PageNumber,

                PageSize =
                    result.PageSize
            };

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

        return ApiResponse<
            PaginatedResult<HallPricingDto>>
            .SuccessResponse(
                response,
                messageHelper.RetrievedEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallPricing),
                HttpStatusCode.OK);
    }
}