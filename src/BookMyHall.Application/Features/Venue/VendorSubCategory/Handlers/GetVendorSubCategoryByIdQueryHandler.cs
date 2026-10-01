using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorSubCategoryByIdQueryHandler(IVendorSubCategoryRepository vendorSubCategoryRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorSubCategoryByIdQuery, ApiResponse<VendorSubCategory>>
{
    public async Task<ApiResponse<VendorSubCategory>> Handle(GetVendorSubCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.VendorSubCategories}:{request.VendorSubCategoryId}";
        var cachedVendorSubCategories = await cacheService.GetAsync<VendorSubCategory>(cacheKey, cancellationToken);
        if (cachedVendorSubCategories is not null)
        {
            return ApiResponse<VendorSubCategory>.SuccessResponse(cachedVendorSubCategories, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.VendorService), HttpStatusCode.OK);
        }

        var vendorSubCategory = await vendorSubCategoryRepository.GetByIdAsync(request.VendorSubCategoryId, cancellationToken);
        if (vendorSubCategory is null)
        {
            return ApiResponse<VendorSubCategory>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities, EntityKeys.VendorSubCategory), HttpStatusCode.NotFound);
        }

        var response = mapper.Map<VendorSubCategory>(vendorSubCategory);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<VendorSubCategory>.SuccessResponse(
            response, messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorSubCategory),
            HttpStatusCode.OK);
    }
}