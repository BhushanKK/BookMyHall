using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetAreaByIdQueryHandler(
    IAreaRepository areaRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetAreaByIdQuery, ApiResponse<AreaDto>>
{
    public async Task<ApiResponse<AreaDto>> Handle(
        GetAreaByIdQuery request,
        CancellationToken cancellationToken)
    {
        var area = await areaRepository.GetByIdAsync(
            request.AreaId,
            cancellationToken);

        if (area is null)
        {
            return ApiResponse<AreaDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Area),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<AreaDto>.SuccessResponse(
            mapper.Map<AreaDto>(area),
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Area),
            HttpStatusCode.OK);
    }
}