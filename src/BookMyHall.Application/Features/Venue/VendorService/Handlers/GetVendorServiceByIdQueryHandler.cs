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

public sealed class GetVendorServiceByIdQueryHandler(IVendorServiceRepository vendorServiceRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorServiceByIdQuery, ApiResponse<VendorService>>
{
    public async Task<ApiResponse<VendorService>> Handle(GetVendorServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.VendorServices}:{request.VendorServiceId}";
        var cachedVendorService = await cacheService.GetAsync<VendorService>(cacheKey, cancellationToken);
        if (cachedVendorService is not null)
        {
            return ApiResponse<VendorService>.SuccessResponse(cachedVendorService, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.VendorService), HttpStatusCode.OK);
        }

        var vendorService = await vendorServiceRepository.GetByIdAsync(request.VendorServiceId, cancellationToken);
        if (vendorService is null)
        {
            return ApiResponse<VendorService>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities, EntityKeys.VendorService), HttpStatusCode.NotFound);
        }

        var response = mapper.Map<VendorService>(vendorService);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<VendorService>.SuccessResponse(
            response, messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorService),
            HttpStatusCode.OK);
    }
}