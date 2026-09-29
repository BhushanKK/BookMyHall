using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorAvailabilityQueryHandler(IVendorAvailabilityRepository vendorAvailabilityRepository,
    IMapper mapper,IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorAvailabilitiesQuery,ApiResponse<PaginatedResponse<VendorAvailabilityDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorAvailabilityDto>>> Handle(GetVendorAvailabilitiesQuery request,
        CancellationToken cancellationToken)
    {
          var pagination = request.Request;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<VendorAvailabilityDto>(
            CacheKeys.VendorAvailabilitiesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<VendorAvailabilityDto>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
          return ApiResponse<PaginatedResponse<VendorAvailabilityDto>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorAvailability),
                HttpStatusCode.OK
            );
        }
        var result =await vendorAvailabilityRepository.GetAllAsync(request.Request,request.VendorId,cancellationToken);
        var response = new PaginatedResponse<VendorAvailabilityDto>
        {
            Items = mapper.Map<IReadOnlyList<VendorAvailabilityDto>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalCount
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<VendorAvailabilityDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorAvailability), HttpStatusCode.OK);
    }
}