using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorSubCategoryByIdQueryHandler(IVendorSubCategoryRepository vendorSubCategoryRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorSubCategoryByIdQuery, ApiResponse<VendorSubCategoryDto>>
{
    public async Task<ApiResponse<VendorSubCategoryDto>> Handle(GetVendorSubCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.VendorSubCategories}:{request.VendorSubCategoryId}";
        var cachedVendorSubCategories = await cacheService.GetAsync<VendorSubCategoryDto>(cacheKey, cancellationToken);
        if (cachedVendorSubCategories is not null)
        {
            return ApiResponse<VendorSubCategoryDto>.SuccessResponse(cachedVendorSubCategories, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.VendorService), HttpStatusCode.OK);
        }

        var vendorSubCategory = await vendorSubCategoryRepository.GetByIdAsync(request.VendorSubCategoryId, cancellationToken);
        if (vendorSubCategory is null)
        {
            return ApiResponse<VendorSubCategoryDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities, EntityKeys.VendorSubCategory), HttpStatusCode.NotFound);
        }

        var response = mapper.Map<VendorSubCategoryDto>(vendorSubCategory);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<VendorSubCategoryDto>.SuccessResponse(
            response, messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorSubCategory),
            HttpStatusCode.OK);
    }
}