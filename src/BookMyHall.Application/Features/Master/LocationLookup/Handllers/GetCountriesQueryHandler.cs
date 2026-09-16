using MediatR;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Master.LocationLookup.Queries;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Application.Master.LocationLookup.Handlers;

public sealed class GetCountriesQueryHandler(ILocationLookupRepository locationLookupRepository,ICacheService cacheService)
    : IRequestHandler<GetCountriesQuery,ApiResponse<IReadOnlyList<LocationLookupDto.CountryLookupDto>>>
{
    public async Task<ApiResponse<IReadOnlyList<LocationLookupDto.CountryLookupDto>>> Handle(GetCountriesQuery request,CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.CountriesCached;
        var cachedCountries = await cacheService.GetAsync<IReadOnlyList<LocationLookupDto.CountryLookupDto>>(cacheKey,cancellationToken);

        if (cachedCountries is not null)
        {
            return ApiResponse<IReadOnlyList<LocationLookupDto.CountryLookupDto>>
                .SuccessResponse(cachedCountries);
        }

        var countries =await locationLookupRepository.GetCountriesAsync(cancellationToken);

        await cacheService.SetAsync(cacheKey,countries,TimeSpan.FromMinutes(30),cancellationToken);

        return ApiResponse<IReadOnlyList<LocationLookupDto.CountryLookupDto>>
            .SuccessResponse(countries);
    }
}