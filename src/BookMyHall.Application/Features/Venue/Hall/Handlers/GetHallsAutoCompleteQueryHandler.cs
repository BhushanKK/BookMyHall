using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;
public sealed class GetHallsAutoCompleteQueryHandler(IHallRepository hallRepository,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetHallsAutoCompleteQuery, ApiResponse<IReadOnlyList<AutoCompleteItem>>>
{
    public async Task<ApiResponse<IReadOnlyList<AutoCompleteItem>>> Handle(
        GetHallsAutoCompleteQuery request, CancellationToken cancellationToken)
    {
        var searchTerm = request.SearchTerm?.Trim();
        var limit = Math.Clamp(request.Limit, 1, 20);
        var cacheKey = $"{CacheKeys.HallsAutoComplete}:{limit}:{searchTerm?.ToLowerInvariant()}";
        var cachedResponse = await cacheService.GetAsync<IReadOnlyList<AutoCompleteItem>>
        (
            cacheKey, 
            cancellationToken
        );

        if (cachedResponse is not null)
        {
            return ApiResponse<IReadOnlyList<AutoCompleteItem>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Hall), 
                HttpStatusCode.OK
            );
        }

        var items = await hallRepository.GetAutoCompleteAsync(searchTerm, limit, cancellationToken);

        await cacheService.SetAsync(cacheKey, items, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<IReadOnlyList<AutoCompleteItem>>.SuccessResponse
        (
            items,
            messageHelper.RetrievedEntity( ResourceNames.Entities, EntityKeys.Hall),
            HttpStatusCode.OK
        );
    }
}