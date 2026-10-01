using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;
public sealed class GetVendorCategoriesQueryHandler(IVendorCategoryRepository vendorCategoryRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorCategoriesQuery, ApiResponse<PaginatedResponse<VendorCategory>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorCategory>>> Handle(GetVendorCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.Request;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<VendorCategory>(
            CacheKeys.VendorCategoriesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<VendorCategory>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
          return ApiResponse<PaginatedResponse<VendorCategory>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorCategory),
                HttpStatusCode.OK
            );
        }
       var pagedResult = await vendorCategoryRepository.GetAllAsync(pagination, cancellationToken);
        var response = new PaginatedResponse<VendorCategory>
        {
            Items =mapper.Map<IReadOnlyList<VendorCategory>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

         await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<VendorCategory>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorCategory),
            HttpStatusCode.OK
        );
    }
}