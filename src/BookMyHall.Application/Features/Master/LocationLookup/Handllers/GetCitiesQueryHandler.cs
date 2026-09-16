using System.Net;
using FluentValidation;
using MediatR;
using BookMyHall.Domain.Dtos;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Master.LocationLookup.Queries;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Master.LocationLookup.Handlers;

public sealed class GetCitiesQueryHandler(ILocationLookupRepository locationLookupRepository,
 IValidator<GetCitiesQuery> validator,ICacheService cacheService)
    : IRequestHandler<GetCitiesQuery,ApiResponse<IReadOnlyList<LocationLookupDto.CityLookupDto>>>
{
    public async Task<ApiResponse<IReadOnlyList<LocationLookupDto.CityLookupDto>>> Handle(GetCitiesQuery request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<IReadOnlyList<LocationLookupDto.CityLookupDto>>
                .FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var cacheKey =$"{CacheKeys.CitiesCached}:{request.DistrictId}";
        var cachedCities =await cacheService.GetAsync<IReadOnlyList<LocationLookupDto.CityLookupDto>>(cacheKey,cancellationToken);

        if (cachedCities is not null)
        {
            return ApiResponse<IReadOnlyList<LocationLookupDto.CityLookupDto>>
                .SuccessResponse(cachedCities);
        }

        var cities =await locationLookupRepository.GetCitiesAsync(request.DistrictId,cancellationToken);
        
        await cacheService.SetAsync(cacheKey,cities,TimeSpan.FromMinutes(30),cancellationToken);

        return ApiResponse<IReadOnlyList<LocationLookupDto.CityLookupDto>>
            .SuccessResponse(cities);
    }
}