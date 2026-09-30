using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorServiceAreaByIdQueryHandler(IVendorServiceAreaRepository vendorServiceAreaRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheServiceArea)
    : IRequestHandler<GetVendorServiceAreaByIdQuery, ApiResponse<VendorServiceAreaDto>>
{
    public async Task<ApiResponse<VendorServiceAreaDto>> Handle(GetVendorServiceAreaByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.VendorServiceAreas}:{request.VendorServiceAreaId}";
        var cachedVendorServiceArea = await cacheServiceArea.GetAsync<VendorServiceAreaDto>(cacheKey, cancellationToken);
        if (cachedVendorServiceArea is not null)
        {
            return ApiResponse<VendorServiceAreaDto>.SuccessResponse(cachedVendorServiceArea, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.VendorServiceArea), HttpStatusCode.OK);
        }

        var vendorServiceArea = await vendorServiceAreaRepository.GetByIdAsync(request.VendorServiceAreaId, cancellationToken);
        if (vendorServiceArea is null)
        {
            return ApiResponse<VendorServiceAreaDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities, EntityKeys.VendorServiceArea), HttpStatusCode.NotFound);
        }

        var response = mapper.Map<VendorServiceAreaDto>(vendorServiceArea);
        await cacheServiceArea.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<VendorServiceAreaDto>.SuccessResponse(
            response, messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorServiceArea),
            HttpStatusCode.OK);
    }
}