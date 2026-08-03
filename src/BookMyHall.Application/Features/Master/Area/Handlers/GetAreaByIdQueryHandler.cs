using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetAreaByIdQueryHandler(
    IAreaRepository areaRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetAreaByIdQuery, ApiResponse<Area>>
{
    public async Task<ApiResponse<Area>> Handle(GetAreaByIdQuery request,CancellationToken cancellationToken)
    {
        var area = await areaRepository.GetByIdAsync(request.AreaId,cancellationToken);
        if (area is null)
        {
            return ApiResponse<Area>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Area),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<Area>.SuccessResponse(
            mapper.Map<Area>(area),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Area),HttpStatusCode.OK);
    }
}