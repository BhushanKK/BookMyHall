using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorServiceByIdQueryHandler(IVendorServiceRepository vendorServiceRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorServiceByIdQuery, ApiResponse<VendorServiceDto>>
{
    public async Task<ApiResponse<VendorServiceDto>> Handle(GetVendorServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.VendorServices}:{request.VendorServiceId}";
        var cachedVendorService = await cacheService.GetAsync<VendorServiceDto>(cacheKey, cancellationToken);
        if (cachedVendorService is not null)
        {
            return ApiResponse<VendorServiceDto>.SuccessResponse(cachedVendorService, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.VendorService), HttpStatusCode.OK);
        }

        var vendorService = await vendorServiceRepository.GetByIdAsync(request.VendorServiceId, cancellationToken);
        if (vendorService is null)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities, EntityKeys.VendorService), HttpStatusCode.NotFound);
        }

        var response = mapper.Map<VendorServiceDto>(vendorService);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<VendorServiceDto>.SuccessResponse(
            response, messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorService),
            HttpStatusCode.OK);
    }
}