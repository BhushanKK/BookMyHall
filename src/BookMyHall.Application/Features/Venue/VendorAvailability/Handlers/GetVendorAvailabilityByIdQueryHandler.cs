using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorAvailabilityByIdQueryHandler(
    IVendorAvailabilityRepository vendorAvailabilityRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        GetVendorAvailabilityByIdQuery,
        ApiResponse<VendorAvailabilityDto>>
{
    public async Task<ApiResponse<VendorAvailabilityDto>> Handle(
        GetVendorAvailabilityByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey =$"{CacheKeys.VendorAvailabilities}:{request.VendorAvailabilityId}";

        var cachedVendorAvailability =await cacheService.GetAsync<VendorAvailabilityDto>(
                cacheKey,
                cancellationToken);

        if (cachedVendorAvailability is not null)
        {
            return ApiResponse<VendorAvailabilityDto>.SuccessResponse(
                cachedVendorAvailability,
                string.Empty,
                HttpStatusCode.OK);
        }

        var vendoravailability =
            await vendorAvailabilityRepository.GetByIdAsync(
                request.VendorAvailabilityId,
                cancellationToken);

        if (vendoravailability is null)
        {
            return ApiResponse<VendorAvailabilityDto>.FailureResponse(
                messageHelper.NotFound(
                    EntityKeys.VendorAvailability),
                HttpStatusCode.NotFound);
        }

        var response =mapper.Map<VendorAvailabilityDto>(vendoravailability);

        await cacheService.SetAsync(cacheKey,response,TimeSpan.FromMinutes(30),cancellationToken);

        return ApiResponse<VendorAvailabilityDto>.SuccessResponse(
            response,
            string.Empty,
            HttpStatusCode.OK);
    }
}