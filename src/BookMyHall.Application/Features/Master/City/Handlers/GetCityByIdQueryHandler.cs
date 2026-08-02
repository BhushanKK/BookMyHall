using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
namespace BookMyHall.Application.Features.Master;


public sealed class GetCityByIdQueryHandler(
    ICityRepository cityRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetCityByIdQuery, ApiResponse<City>>
{
    public async Task<ApiResponse<City>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
    {
        var city = await cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city is null)
        {
            return ApiResponse<City>.FailureResponse(messageHelper.NotFound(EntityKeys.City), HttpStatusCode.NotFound);
        }

        return ApiResponse<City>.SuccessResponse(
            mapper.Map<City>(city),
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.City), HttpStatusCode.OK);
    }
}