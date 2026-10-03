using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorServicesQueryHandler(IVendorServiceRepository vendorServiceRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorServicesQuery, ApiResponse<PaginatedResponse<VendorServiceDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorServiceDto>>> Handle(GetVendorServicesQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.Request;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<VendorServiceDto>(
            CacheKeys.VendorServicesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending) + $":v2:{request.VendorId}:{request.VendorCategoryId}:{request.VendorSubCategoryId}";

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<VendorServiceDto>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<VendorServiceDto>>.SuccessResponse
              (
                  cachedResponse,
                  messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorService),
                  HttpStatusCode.OK
              );
        }
        
        var result = await vendorServiceRepository.GetAllAsync(pagination,
        request.VendorId,request.VendorCategoryId, request.VendorSubCategoryId, cancellationToken);
        var response = new PaginatedResponse<VendorServiceDto>
        {
            Items = mapper.Map<IReadOnlyList<VendorServiceDto>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalCount
        };
        
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<VendorServiceDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorService), HttpStatusCode.OK);
    }
}
