using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorsQueryHandler(IVendorRepository vendorRepository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorsQuery,ApiResponse<PaginatedResponse<VendorDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorDto>>> Handle(GetVendorsQuery request,CancellationToken cancellationToken)
    {
        var pagination = request.Request;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<VendorDto>(
            CacheKeys.VendorsPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<VendorDto>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<VendorDto>>.SuccessResponse
              (
                  cachedResponse,
                  messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Vendor),
                  HttpStatusCode.OK
              );
        }
        var result = await vendorRepository.GetAllAsync(pagination, cancellationToken);

        var response = new PaginatedResponse<VendorDto>
        {
            Items = mapper.Map<IReadOnlyList<VendorDto>>(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalCount
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<VendorDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Vendor), HttpStatusCode.OK);

    }
}