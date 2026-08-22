using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCountryByIdQueryHandler(
    ICountryRepository countryRepository, IMessageHelper messageHelper,
    IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetCountryByIdQuery, ApiResponse<Country>>
{
    public async Task<ApiResponse<Country>> Handle(
        GetCountryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Country}:{request.CountryId}";
        var cachedCountry = await cacheService.GetAsync<Country>(cacheKey, cancellationToken);
        
        if (cachedCountry is not null)
        {
            return ApiResponse<Country>.SuccessResponse
            (
                cachedCountry,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Country),
                HttpStatusCode.OK
            );
        }

        var country = await countryRepository.GetByIdAsync(request.CountryId, cancellationToken);

        if (country is null)
        {
            return ApiResponse<Country>.FailureResponse
            (
                messageHelper.NotFound(EntityKeys.Country),
                HttpStatusCode.NotFound
            );
        }

        var response = mapper.Map<Country>(country);

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<Country>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Country),
            HttpStatusCode.OK
        );
    }
}