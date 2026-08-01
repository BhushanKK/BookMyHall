using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetEventCategoryByIdQueryHandler(
    IEventCategoryRepository eventCategoryRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetEventCategoryByIdQuery, ApiResponse<EventCategoryDto>>
{
    public async Task<ApiResponse<EventCategoryDto>> Handle(GetEventCategoryByIdQuery request,CancellationToken cancellationToken)
    {
        var eventCategory = await eventCategoryRepository.GetByIdAsync(
            request.EventCategoryId,
            cancellationToken);

        if (eventCategory is null)
        {
            return ApiResponse<EventCategoryDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.EventCategory),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<EventCategoryDto>.SuccessResponse(
            mapper.Map<EventCategoryDto>(eventCategory),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.EventCategory),HttpStatusCode.OK);
    }
}