using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetEventCategoryByIdQueryHandler(
    IEventCategoryRepository eventCategoryRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetEventCategoryByIdQuery, ApiResponse<EventCategory>>
{
    public async Task<ApiResponse<EventCategory>> Handle(GetEventCategoryByIdQuery request,CancellationToken cancellationToken)
    {
        var eventCategory = await eventCategoryRepository.GetByIdAsync(request.EventCategoryId,cancellationToken);

        if (eventCategory is null)
        {
            return ApiResponse<EventCategory>.FailureResponse(
                messageHelper.NotFound(EntityKeys.EventCategory),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<EventCategory>.SuccessResponse(
            mapper.Map<EventCategory>(eventCategory),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.EventCategory),HttpStatusCode.OK);
    }
}