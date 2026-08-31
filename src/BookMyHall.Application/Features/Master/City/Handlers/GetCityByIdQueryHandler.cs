using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Caching;
namespace BookMyHall.Application.Features.Master;


public sealed class GetCityByIdQueryHandler(
    ICityRepository cityRepository,
    IMessageHelper messageHelper,
    IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetCityByIdQuery, ApiResponse<City>>
{
    public async Task<ApiResponse<City>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Cities}:{request.CityId}";
        var cachedCity = await cacheService.GetAsync<City>(cacheKey, cancellationToken);
        if (cachedCity is not null)
        {
            return ApiResponse<City>.SuccessResponse(cachedCity, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.City), HttpStatusCode.OK);
        }
        var city = await cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city is null)
        {
            return ApiResponse<City>.FailureResponse(messageHelper.NotFound(EntityKeys.City), HttpStatusCode.NotFound);
        }
        var response = mapper.Map<City>(city);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<City>.SuccessResponse(
            mapper.Map<City>(city),
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.City), HttpStatusCode.OK);
    }
}