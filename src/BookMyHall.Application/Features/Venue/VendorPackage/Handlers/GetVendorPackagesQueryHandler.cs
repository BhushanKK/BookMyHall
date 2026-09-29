using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Venue;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorPackagesQueryHandler(IVendorPackageRepository repository,
    IMapper mapper, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorPackagesQuery, ApiResponse<PaginatedResponse<VendorPackage>>>
{
    public async Task<ApiResponse<PaginatedResponse<VendorPackage>>> Handle(GetVendorPackagesQuery request, CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<VendorPackage>(
            CacheKeys.VendorPackagesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<VendorPackage>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<VendorPackage>>.SuccessResponse(cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorPackage), HttpStatusCode.OK);
        }

        var pagedResult = await repository.GetAllAsync(pagination, cancellationToken);

        var response = new PaginatedResponse<VendorPackage>
        {
            Items = mapper.Map<IReadOnlyList<VendorPackage>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<VendorPackage>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorPackage), HttpStatusCode.OK);
    }
}
