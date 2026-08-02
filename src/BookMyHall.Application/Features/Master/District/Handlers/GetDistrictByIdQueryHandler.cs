using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetDistrictByIdQueryHandler(
    IDistrictRepository districtRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetDistrictByIdQuery, ApiResponse<District>>
{
    public async Task<ApiResponse<District>> Handle(GetDistrictByIdQuery request,CancellationToken cancellationToken)
    {
        var district = await districtRepository.GetByIdAsync(request.DistrictId,cancellationToken);

        if (district is null)
        {
            return ApiResponse<District>.FailureResponse(
                messageHelper.NotFound(EntityKeys.District),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<District>.SuccessResponse(
            mapper.Map<District>(district),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.District),HttpStatusCode.OK);
    }
}