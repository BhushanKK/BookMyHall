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

public sealed class GetVendorServiceAreasQueryHandler(IVendorServiceAreaRepository vendorServiceAreaRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheServiceArea)
    : IRequestHandler<GetVendorServiceAreasQuery, ApiResponse<PaginatedResponse<VendorServiceArea>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorServiceArea>>> Handle(GetVendorServiceAreasQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.Pagination;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<VendorServiceArea>(
            CacheKeys.VendorServiceAreasPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheServiceArea.GetAsync<PaginatedResponse<VendorServiceArea>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<VendorServiceArea>>.SuccessResponse
              (
                  cachedResponse,
                  messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorServiceArea),
                  HttpStatusCode.OK
              );
        }
        
        var result = await vendorServiceAreaRepository.GetAllAsync(pagination,request.VendorId, cancellationToken);
        var response = new PaginatedResponse<VendorServiceArea>
        {
            Items = mapper.Map<IReadOnlyList<VendorServiceArea>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalCount
        };
        
        await cacheServiceArea.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<VendorServiceArea>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorServiceArea), HttpStatusCode.OK);
    }
}