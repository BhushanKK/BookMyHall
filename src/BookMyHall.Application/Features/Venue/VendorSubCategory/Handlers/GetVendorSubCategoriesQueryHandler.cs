using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorSubCategoriesQueryHandler(IVendorSubCategoryRepository vendorSubCategoryRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorSubCategoriesQuery, ApiResponse<PaginatedResponse<VendorSubCategoryDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorSubCategoryDto>>> Handle(GetVendorSubCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.Request;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<VendorSubCategoryDto>(
            CacheKeys.VendorServicesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<VendorSubCategoryDto>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<VendorSubCategoryDto>>.SuccessResponse
              (
                  cachedResponse,
                  messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorSubCategory),
                  HttpStatusCode.OK
              );
        }
        var result = await vendorSubCategoryRepository.GetAllAsync(pagination, request.VendorCategoryId, cancellationToken);
        var response = new PaginatedResponse<VendorSubCategoryDto>
        {
            Items = mapper.Map<IReadOnlyList<VendorSubCategoryDto>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalCount
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<VendorSubCategoryDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorSubCategory), HttpStatusCode.OK);
    }
}