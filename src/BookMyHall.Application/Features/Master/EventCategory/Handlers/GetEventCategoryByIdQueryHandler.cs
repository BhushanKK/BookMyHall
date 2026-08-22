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

public sealed class GetEventCategoryByIdQueryHandler(
    IEventCategoryRepository eventCategoryRepository,
    IMessageHelper messageHelper,
    IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetEventCategoryByIdQuery, ApiResponse<EventCategory>>
{
    public async Task<ApiResponse<EventCategory>> Handle(GetEventCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.EventCategory}:{request.EventCategoryId}";
        var cachedEventCategory = await cacheService.GetAsync<EventCategory>(cacheKey, cancellationToken);

        if (cachedEventCategory is not null)
        {
            return ApiResponse<EventCategory>.SuccessResponse
            (
                cachedEventCategory,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.EventCategory),
                HttpStatusCode.OK
            );
        }

        var eventCategory = await eventCategoryRepository.GetByIdAsync(request.EventCategoryId, cancellationToken);

        if (eventCategory is null)
        {
            return ApiResponse<EventCategory>.FailureResponse(
                messageHelper.NotFound(EntityKeys.EventCategory),
                HttpStatusCode.NotFound);
        }
        var response = mapper.Map<EventCategory>(eventCategory);

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<EventCategory>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.EventCategory), HttpStatusCode.OK);
    }
}