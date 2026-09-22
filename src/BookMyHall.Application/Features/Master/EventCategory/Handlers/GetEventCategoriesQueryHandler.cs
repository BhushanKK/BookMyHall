using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class GetEventCategoriesQueryHandler(IEventCategoryRepository eventCategoryRepository,
    IMessageHelper messageHelper,IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetEventCategoriesQuery, ApiResponse<PaginatedResponse<EventCategory>>>
{
    public async Task<ApiResponse<PaginatedResponse<EventCategory>>> Handle(GetEventCategoriesQuery request, CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<EventCategory>(
            CacheKeys.EventCategoriesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<EventCategory>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<EventCategory>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.EventCategory),
                HttpStatusCode.OK
            );
        }

        var result = await eventCategoryRepository.GetAllAsync(request.paginationRequest, cancellationToken);
        var response = new PaginatedResponse<EventCategory>
        {
            Items = mapper.Map<IReadOnlyList<EventCategory>>(result.Items),
            TotalRecords = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<PaginatedResponse<EventCategory>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.EventCategory), HttpStatusCode.OK);
    }
}