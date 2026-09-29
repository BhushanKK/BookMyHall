using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorSubCategoryServicesAutoCompleteQueryHandler(IVendorSubCategoryRepository repository,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetVendorSubCategoriesAutoCompleteQuery, ApiResponse<IReadOnlyList<AutoCompleteItem>>>
{
    public async Task<ApiResponse<IReadOnlyList<AutoCompleteItem>>> Handle(
        GetVendorSubCategoriesAutoCompleteQuery request, CancellationToken cancellationToken)
    {
        var searchTerm = request.SearchTerm?.Trim();
        var limit = Math.Clamp(request.Limit, 1, 20);

        var cacheKey = $"{CacheKeys.VendorSubCategoriesAutoComplete}:{limit}:{searchTerm?.ToLowerInvariant()}";
        var cachedResponse = await cacheService.GetAsync<IReadOnlyList<AutoCompleteItem>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<IReadOnlyList<AutoCompleteItem>>.SuccessResponse(cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorSubCategory), HttpStatusCode.OK);
        }

        var items = await repository.GetAutoCompleteAsync(searchTerm, limit, cancellationToken);
        await cacheService.SetAsync(cacheKey, items, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<IReadOnlyList<AutoCompleteItem>>.SuccessResponse(items,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorSubCategory), HttpStatusCode.OK);
    }
}
