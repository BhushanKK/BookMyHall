using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetAreasQueryHandler(
    IAreaRepository areaRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetAreasQuery, ApiResponse<PaginatedResult<AreaDto>>>
{
    public async Task<ApiResponse<PaginatedResult<AreaDto>>> Handle(
        GetAreasQuery request,
        CancellationToken cancellationToken)
    {
        var result = await areaRepository.GetAllAsync(
            request.PaginationRequest,
            cancellationToken);

        var response = new PaginatedResult<AreaDto>
        {
            Items = mapper.Map<List<AreaDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<AreaDto>>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Area),
            HttpStatusCode.OK);
    }
}