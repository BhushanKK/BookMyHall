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

public sealed class GetVendorAvailabilityByIdQueryHandler(IVendorAvailabilityRepository vendorAvailabilityRepository,
    IMapper mapper,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetVendorAvailabilityByIdQuery,ApiResponse<VendorAvailability>>
{
    public async Task<ApiResponse<VendorAvailability>> Handle(GetVendorAvailabilityByIdQuery request,CancellationToken cancellationToken)
    {
        var cacheKey =$"{CacheKeys.VendorAvailabilities}:{request.VendorAvailabilityId}";
        var cachedVendorAvailability =await cacheService.GetAsync<VendorAvailability>(cacheKey, cancellationToken);
        if (cachedVendorAvailability is not null)
        {
            return ApiResponse<VendorAvailability>.SuccessResponse(cachedVendorAvailability,
                string.Empty,
                HttpStatusCode.OK);
        }

        var vendoravailability =await vendorAvailabilityRepository.GetByIdAsync(request.VendorAvailabilityId,cancellationToken);
        if (vendoravailability is null)
        {
            return ApiResponse<VendorAvailability>.FailureResponse(messageHelper.NotFound(
                    EntityKeys.VendorAvailability),HttpStatusCode.NotFound);
        }

        var response =mapper.Map<VendorAvailability>(vendoravailability);
        await cacheService.SetAsync(cacheKey,response,TimeSpan.FromMinutes(30),cancellationToken);
        return ApiResponse<VendorAvailability>.SuccessResponse
        (response,messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorAvailability),
            HttpStatusCode.OK);
    }
}