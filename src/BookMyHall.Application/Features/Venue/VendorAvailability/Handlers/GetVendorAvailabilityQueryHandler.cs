using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorAvailabilityQueryHandler(IVendorAvailabilityRepository vendorAvailabilityRepository,
    IMapper mapper,IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorAvailabilitiesQuery,ApiResponse<PaginatedResponse<VendorAvailability>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorAvailability>>> Handle(GetVendorAvailabilitiesQuery request,
        CancellationToken cancellationToken)
    {
          var pagination = request.Request;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<VendorAvailability>(
            CacheKeys.VendorAvailabilitiesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<VendorAvailability>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
          return ApiResponse<PaginatedResponse<VendorAvailability>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorAvailability),
                HttpStatusCode.OK
            );
        }
        var result =await vendorAvailabilityRepository.GetAllAsync(request.Request,request.VendorId,cancellationToken);
        var response = new PaginatedResponse<VendorAvailability>
        {
            Items = mapper.Map<IReadOnlyList<VendorAvailability>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalCount
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<VendorAvailability>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorAvailability), HttpStatusCode.OK);
    }
}