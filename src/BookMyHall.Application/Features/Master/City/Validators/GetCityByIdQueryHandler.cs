using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCityByIdQueryHandler(
    ICityRepository cityRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetCityByIdQuery, ApiResponse<CityDto>>
{
    public async Task<ApiResponse<CityDto>> Handle(GetCityByIdQuery request,CancellationToken cancellationToken)
    {
        var city = await cityRepository.GetByIdAsync(request.CityId,cancellationToken);
        if (city is null)
        {
            return ApiResponse<CityDto>.FailureResponse(messageHelper.NotFound(EntityKeys.City),HttpStatusCode.NotFound);
        }

        return ApiResponse<CityDto>.SuccessResponse(
            mapper.Map<CityDto>(city),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.City),HttpStatusCode.OK);
    }
}