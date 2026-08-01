using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetDistrictByIdQueryHandler(
    IDistrictRepository districtRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetDistrictByIdQuery, ApiResponse<DistrictDto>>
{
    public async Task<ApiResponse<DistrictDto>> Handle(GetDistrictByIdQuery request,CancellationToken cancellationToken)
    {
        var district = await districtRepository.GetByIdAsync(request.DistrictId,cancellationToken);

        if (district is null)
        {
            return ApiResponse<DistrictDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.District),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<DistrictDto>.SuccessResponse(
            mapper.Map<DistrictDto>(district),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.District),HttpStatusCode.OK);
    }
}