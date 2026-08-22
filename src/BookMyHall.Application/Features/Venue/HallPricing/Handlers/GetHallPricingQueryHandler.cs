using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;
public sealed class GetHallPricingQueryHandler(IHallPricingRepository hallPricingRepository,
    IMessageHelper messageHelper,
    IMapper mapper,ICacheService cacheService)
    : IRequestHandler<GetHallPricingQuery,ApiResponse<PaginatedResult<HallPricingDto>>>
{
    public async Task<ApiResponse<PaginatedResult<HallPricingDto>>> Handle(GetHallPricingQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<HallPricing>(
            CacheKeys.HallPricing,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResult<HallPricingDto>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResult<HallPricingDto>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallPricing),
                HttpStatusCode.OK
            );
        }
        var result = await hallPricingRepository.GetAllAsync(request.paginationRequest, cancellationToken);

        var response = new PaginatedResult<HallPricingDto>
        {
            Items = mapper.Map<IReadOnlyList<HallPricingDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<PaginatedResult<HallPricingDto>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.HallPricing),
            HttpStatusCode.OK
        );
    }
}