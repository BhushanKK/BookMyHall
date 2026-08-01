using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetFacilityByIdQueryHandler(
    IFacilityRepository facilityRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetFacilityByIdQuery, ApiResponse<FacilityDto>>
{
    public async Task<ApiResponse<FacilityDto>> Handle(GetFacilityByIdQuery request,CancellationToken cancellationToken)
    {
        var facility = await facilityRepository.GetByIdAsync(request.FacilityId,cancellationToken);

        if (facility is null)
        {
            return ApiResponse<FacilityDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Facility),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<FacilityDto>.SuccessResponse(
            mapper.Map<FacilityDto>(facility),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Facility),HttpStatusCode.OK);
    }
}