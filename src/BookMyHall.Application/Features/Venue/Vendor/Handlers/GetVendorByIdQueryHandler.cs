using MediatR;
using System.Net;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorByIdQueryHandler(IVendorRepository vendorRepository,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorByIdQuery, ApiResponse<Vendor>>
{
    public async Task<ApiResponse<Vendor>> Handle(
        GetVendorByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.VendorId == Guid.Empty)
        {
            return ApiResponse<Vendor>.FailureResponse
            (
                messageHelper.NotFound(EntityKeys.Vendor),
                HttpStatusCode.NotFound
            );
        }

        var cacheKey = $"{CacheKeys.Vendors}:{request.VendorId}";

        var cachedVendor = await cacheService.GetAsync<Vendor>(cacheKey, cancellationToken);

        if (cachedVendor is not null)
        {
            return ApiResponse<Vendor>.SuccessResponse
            (
                cachedVendor,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Vendor),
                HttpStatusCode.OK
            );
        }

        var vendor = await vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor is null)
        {
            return ApiResponse<Vendor>.FailureResponse
            (
                messageHelper.NotFound(EntityKeys.Vendor),
                HttpStatusCode.NotFound
            );
        }

        await cacheService.SetAsync(cacheKey, vendor, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<Vendor>.SuccessResponse
        (
            vendor,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Vendor),
            HttpStatusCode.OK
        );
    }
}