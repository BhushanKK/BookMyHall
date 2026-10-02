using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorsQueryHandler(IVendorRepository vendorRepository,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorsQuery, ApiResponse<PaginatedResponse<VendorListView>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorListView>>> Handle(
        GetVendorsQuery request, CancellationToken cancellationToken)
    {
        var pagination = request.Request;

        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<Vendor>
        (
            CacheKeys.VendorsPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending
        );

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<VendorListView>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<VendorListView>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Vendor),
                HttpStatusCode.OK
            );
        }

        var result = await vendorRepository.GetAllAsync(pagination, cancellationToken);

        var response = new PaginatedResponse<VendorListView>
        {
            Items = result.Items,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalCount
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<PaginatedResponse<VendorListView>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Vendor),
            HttpStatusCode.OK
        );
    }
}