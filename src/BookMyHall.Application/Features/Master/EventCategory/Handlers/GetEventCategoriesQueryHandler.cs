using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetEventCategoriesQueryHandler(
    IEventCategoryRepository eventCategoryRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetEventCategoriesQuery, ApiResponse<PaginatedResult<EventCategory>>>
{
    public async Task<ApiResponse<PaginatedResult<EventCategory>>> Handle(GetEventCategoriesQuery request,CancellationToken cancellationToken)
    {
        var result = await eventCategoryRepository.GetAllAsync(request.paginationRequest,cancellationToken);
        var response = new PaginatedResult<EventCategory>
        {
            Items = mapper.Map<IReadOnlyList<EventCategory>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<EventCategory>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.EventCategory),HttpStatusCode.OK);
    }
}