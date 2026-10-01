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

public sealed class GetVendorPackageItemsQueryHandler(IVendorPackageItemRepository vendorPackageItemRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheServiceArea)
    : IRequestHandler<GetVendorPackageItemsQuery, ApiResponse<PaginatedResponse<VendorPackageItem>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorPackageItem>>> Handle(GetVendorPackageItemsQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.Pagination;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<VendorPackageItem>(
            CacheKeys.VendorPackageItemsPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheServiceArea.GetAsync<PaginatedResponse<VendorPackageItem>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<VendorPackageItem>>.SuccessResponse
              (
                  cachedResponse,
                  messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorPackageItem),
                  HttpStatusCode.OK
              );
        }
        
        var result = await vendorPackageItemRepository.GetAllAsync(pagination,request.VendorPackageId, cancellationToken);
        var response = new PaginatedResponse<VendorPackageItem>
        {
            Items = mapper.Map<IReadOnlyList<VendorPackageItem>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalCount
        };
        
        await cacheServiceArea.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<VendorPackageItem>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorPackageItem), HttpStatusCode.OK);
    }
}