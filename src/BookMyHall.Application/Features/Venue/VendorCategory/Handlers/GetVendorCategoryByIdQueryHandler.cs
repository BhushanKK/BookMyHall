using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorCategoryByIdQueryHandler(IVendorCategoryRepository vendorCategoryRepository,
    IMapper mapper,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetVendorCategoryByIdQuery,ApiResponse<VendorCategory>>
{
    public async Task<ApiResponse<VendorCategory>> Handle(GetVendorCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey =$"{CacheKeys.VendorCategories}:{request.VendorCategoryId}";
        var cachedVendorCategory =await cacheService.GetAsync<VendorCategory>(cacheKey,cancellationToken);
        if (cachedVendorCategory is not null)
        {
            return ApiResponse<VendorCategory>.SuccessResponse(cachedVendorCategory,
                string.Empty,HttpStatusCode.OK);
        }

        var vendorcategory =await vendorCategoryRepository.GetByIdAsync(request.VendorCategoryId,cancellationToken);

        if (vendorcategory is null)
        {
            return ApiResponse<VendorCategory>.FailureResponse(messageHelper.NotFound(
                    EntityKeys.VendorCategory),HttpStatusCode.NotFound);
        }

        var response =mapper.Map<VendorCategory>(vendorcategory);
        await cacheService.SetAsync(cacheKey,response,TimeSpan.FromMinutes(30),cancellationToken);
         return ApiResponse<VendorCategory>.SuccessResponse
        (response,messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorCategory),
            HttpStatusCode.OK
        );
    }
}