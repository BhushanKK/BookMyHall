using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorPackageItemByIdQueryHandler(IVendorPackageItemRepository vendorPackageItemRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheVendorPackageItem)
    : IRequestHandler<GetVendorPackageItemByIdQuery, ApiResponse<VendorPackageItemDto>>
{
    public async Task<ApiResponse<VendorPackageItemDto>> Handle(GetVendorPackageItemByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.VendorPackageItems}:{request.VendorPackageItemId}";
        var cachedVendorPackageItem = await cacheVendorPackageItem.GetAsync<VendorPackageItemDto>(cacheKey, cancellationToken);
        if (cachedVendorPackageItem is not null)
        {
            return ApiResponse<VendorPackageItemDto>.SuccessResponse(cachedVendorPackageItem, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.VendorPackageItem), HttpStatusCode.OK);
        }

        var vendorPackageItem = await vendorPackageItemRepository.GetByIdAsync(request.VendorPackageItemId, cancellationToken);
        if (vendorPackageItem is null)
        {
            return ApiResponse<VendorPackageItemDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities, EntityKeys.VendorPackageItem), HttpStatusCode.NotFound);
        }

        var response = mapper.Map<VendorPackageItemDto>(vendorPackageItem);
        await cacheVendorPackageItem.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<VendorPackageItemDto>.SuccessResponse(
            response, messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorPackageItem),
            HttpStatusCode.OK);
    }
}