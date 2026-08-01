using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetEventCategoriesQueryHandler(
    IEventCategoryRepository eventCategoryRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetEventCategoriesQuery, ApiResponse<PaginatedResult<EventCategoryDto>>>
{
    public async Task<ApiResponse<PaginatedResult<EventCategoryDto>>> Handle(GetEventCategoriesQuery request,CancellationToken cancellationToken)
    {
        var result = await eventCategoryRepository.GetAllAsync(request.PaginationRequest,cancellationToken);
        var response = new PaginatedResult<EventCategoryDto>
        {
            Items = mapper.Map<List<EventCategoryDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<EventCategoryDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.EventCategory),HttpStatusCode.OK);
    }
}