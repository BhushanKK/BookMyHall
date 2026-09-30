using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorServiceAreasQueryHandler(IVendorServiceAreaRepository vendorServiceAreaRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheServiceArea)
    : IRequestHandler<GetVendorServiceAreasQuery, ApiResponse<PaginatedResponse<VendorServiceAreaDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorServiceAreaDto>>> Handle(GetVendorServiceAreasQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.Pagination;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<VendorServiceAreaDto>(
            CacheKeys.VendorServiceAreasPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheServiceArea.GetAsync<PaginatedResponse<VendorServiceAreaDto>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<VendorServiceAreaDto>>.SuccessResponse
              (
                  cachedResponse,
                  messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorServiceArea),
                  HttpStatusCode.OK
              );
        }
        
        var result = await vendorServiceAreaRepository.GetAllAsync(pagination,request.VendorId, cancellationToken);
        var response = new PaginatedResponse<VendorServiceAreaDto>
        {
            Items = mapper.Map<IReadOnlyList<VendorServiceAreaDto>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalCount
        };
        
        await cacheServiceArea.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<VendorServiceAreaDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorServiceArea), HttpStatusCode.OK);
    }
}