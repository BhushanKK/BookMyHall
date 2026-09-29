using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;
public sealed class GetVendorCategoriesQueryHandler(IVendorCategoryRepository vendorCategoryRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorCategoriesQuery, ApiResponse<PaginatedResponse<VendorCategoryDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorCategoryDto>>> Handle(GetVendorCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.Request;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<VendorCategoryDto>(
            CacheKeys.VendorCategoriesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<VendorCategoryDto>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
          return ApiResponse<PaginatedResponse<VendorCategoryDto>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorCategory),
                HttpStatusCode.OK
            );
        }
       var pagedResult = await vendorCategoryRepository.GetAllAsync(pagination, cancellationToken);
        var response = new PaginatedResponse<VendorCategoryDto>
        {
            Items =mapper.Map<IReadOnlyList<VendorCategoryDto>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

         await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<VendorCategoryDto>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorCategory),
            HttpStatusCode.OK
        );
    }
}