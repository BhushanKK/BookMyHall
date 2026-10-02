using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorAutoCompleteQueryHandler(IVendorRepository vendorRepository,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorAutoCompleteQuery, ApiResponse<IReadOnlyList<AutoCompleteItem>>>
{
    public async Task<ApiResponse<IReadOnlyList<AutoCompleteItem>>> Handle(
        GetVendorAutoCompleteQuery request,
        CancellationToken cancellationToken)
    {
        var searchTerm = request.SearchTerm?.Trim();
        var normalizedSearchTerm = searchTerm?.ToLowerInvariant() ?? string.Empty;
        var limit = Math.Clamp(request.Limit, 1, 20);

        var cacheKey = $"{CacheKeys.VendorAutoComplete}:{limit}:{normalizedSearchTerm}:{request.AreaId}";
        var cachedItems = await cacheService.GetAsync<IReadOnlyList<AutoCompleteItem>>(cacheKey, cancellationToken);

        if (cachedItems is not null)
        {
            return ApiResponse<IReadOnlyList<AutoCompleteItem>>.SuccessResponse
            (
                cachedItems,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Vendor),
                HttpStatusCode.OK
            );
        }

        var items = await vendorRepository.GetAutoCompleteAsync(searchTerm, request.AreaId, limit, cancellationToken);

        await cacheService.SetAsync
        (
            cacheKey,
            items,
            TimeSpan.FromMinutes(30),
            cancellationToken
        );

        return ApiResponse<IReadOnlyList<AutoCompleteItem>>.SuccessResponse
        (
            items,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Vendor),
            HttpStatusCode.OK
        );
    }
}