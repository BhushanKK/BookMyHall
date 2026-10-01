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

public sealed class GetVendorServiceAreaByIdQueryHandler(IVendorServiceAreaRepository vendorServiceAreaRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheServiceArea)
    : IRequestHandler<GetVendorServiceAreaByIdQuery, ApiResponse<VendorServiceArea>>
{
    public async Task<ApiResponse<VendorServiceArea>> Handle(GetVendorServiceAreaByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.VendorServiceAreas}:{request.VendorServiceAreaId}";
        var cachedVendorServiceArea = await cacheServiceArea.GetAsync<VendorServiceArea>(cacheKey, cancellationToken);
        if (cachedVendorServiceArea is not null)
        {
            return ApiResponse<VendorServiceArea>.SuccessResponse(cachedVendorServiceArea, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.VendorServiceArea), HttpStatusCode.OK);
        }

        var vendorServiceArea = await vendorServiceAreaRepository.GetByIdAsync(request.VendorServiceAreaId, cancellationToken);
        if (vendorServiceArea is null)
        {
            return ApiResponse<VendorServiceArea>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities, EntityKeys.VendorServiceArea), HttpStatusCode.NotFound);
        }

        var response = mapper.Map<VendorServiceArea>(vendorServiceArea);
        await cacheServiceArea.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<VendorServiceArea>.SuccessResponse(
            response, messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorServiceArea),
            HttpStatusCode.OK);
    }
}