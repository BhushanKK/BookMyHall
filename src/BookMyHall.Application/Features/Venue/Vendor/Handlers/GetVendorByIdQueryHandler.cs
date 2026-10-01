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

public sealed class GetVendorByIdQueryHandler(IVendorRepository vendorRepository,
    IMapper mapper,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetVendorByIdQuery,ApiResponse<Vendors>>
{
    public async Task<ApiResponse<Vendors>> Handle(GetVendorByIdQuery request,CancellationToken cancellationToken)
    {
         var cacheKey = $"{CacheKeys.Vendors}:{request.VendorId}";
        var cachedVendor = await cacheService.GetAsync<Vendors>(cacheKey, cancellationToken);
        if (cachedVendor is not null)
        {
            return ApiResponse<Vendors>.SuccessResponse(cachedVendor, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.Vendor), HttpStatusCode.OK);
        }
        var vendor = await vendorRepository.GetByIdAsync(request.VendorId,cancellationToken);
        if (vendor is null)
        {
            return ApiResponse<Vendors>.FailureResponse(messageHelper.NotFound(
                    EntityKeys.Vendor),HttpStatusCode.NotFound);
        }

        var response =mapper.Map<Vendors>(vendor);
        await cacheService.SetAsync(cacheKey,response,TimeSpan.FromMinutes(30),cancellationToken);
        return ApiResponse<Vendors>.SuccessResponse(
            response,messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Vendor),
            HttpStatusCode.OK);
    }
}