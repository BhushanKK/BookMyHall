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

public sealed class GetHallCategoriesQueryHandler(IHallCategoryRepository hallCategoryRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetHallCategoriesQuery, ApiResponse<PaginatedResult<HallCategory>>>
{
    public async Task<ApiResponse<PaginatedResult<HallCategory>>> Handle(GetHallCategoriesQuery request, CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;

        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<HallCategory>(
            CacheKeys.HallCategories,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResult<HallCategory>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResult<HallCategory>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallCategory),
                HttpStatusCode.OK
            );
        }
        var pagedResult = await hallCategoryRepository.GetAllAsync(request.paginationRequest, cancellationToken);
        var response = new PaginatedResult<HallCategory>
        {
            Items = mapper.Map<IReadOnlyList<HallCategory>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalCount = pagedResult.TotalCount
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<PaginatedResult<HallCategory>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallCategory), HttpStatusCode.OK);
    }
}