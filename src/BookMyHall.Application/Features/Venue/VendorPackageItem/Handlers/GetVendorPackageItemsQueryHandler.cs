using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorPackageItemsQueryHandler(IVendorPackageItemRepository vendorPackageItemRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheServiceArea)
    : IRequestHandler<GetVendorPackageItemsQuery, ApiResponse<PaginatedResponse<VendorPackageItemDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorPackageItemDto>>> Handle(GetVendorPackageItemsQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.Pagination;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<VendorPackageItemDto>(
            CacheKeys.VendorPackageItemsPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheServiceArea.GetAsync<PaginatedResponse<VendorPackageItemDto>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<VendorPackageItemDto>>.SuccessResponse
              (
                  cachedResponse,
                  messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorPackageItem),
                  HttpStatusCode.OK
              );
        }
        
        var result = await vendorPackageItemRepository.GetAllAsync(pagination,request.VendorPackageId, cancellationToken);
        var response = new PaginatedResponse<VendorPackageItemDto>
        {
            Items = mapper.Map<IReadOnlyList<VendorPackageItemDto>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalCount
        };
        
        await cacheServiceArea.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<VendorPackageItemDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorPackageItem), HttpStatusCode.OK);
    }
}