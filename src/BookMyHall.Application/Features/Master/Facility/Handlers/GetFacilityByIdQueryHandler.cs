using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetFacilityByIdQueryHandler(
    IFacilityRepository facilityRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetFacilityByIdQuery, ApiResponse<Facility>>
{
    public async Task<ApiResponse<Facility>> Handle(GetFacilityByIdQuery request,CancellationToken cancellationToken)
    {
        var facility = await facilityRepository.GetByIdAsync(request.FacilityId,cancellationToken);
        if (facility is null)
        {
            return ApiResponse<Facility>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Facility),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<Facility>.SuccessResponse(
            mapper.Map<Facility>(facility),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Facility),HttpStatusCode.OK);
    }
}