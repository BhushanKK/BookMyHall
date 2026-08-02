using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetAreasQueryHandler(
    IAreaRepository areaRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetAreasQuery, ApiResponse<PaginatedResult<Area>>>
{
    public async Task<ApiResponse<PaginatedResult<Area>>> Handle(GetAreasQuery request,CancellationToken cancellationToken)
    {
        var result = await areaRepository.GetAllAsync(request.paginationRequest,cancellationToken);
        var response = new PaginatedResult<Area>
        {
            Items = mapper.Map<IReadOnlyList<Area>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<Area>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Area),HttpStatusCode.OK);
    }
}