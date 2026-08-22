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

public sealed class GetCountriesQueryHandler(
    ICountryRepository countryRepository,
    IMessageHelper messageHelper,
    IMapper mapper,
    ICacheService cacheService)
    : IRequestHandler<GetCountriesQuery, ApiResponse<PaginatedResult<Country>>>
{
    public async Task<ApiResponse<PaginatedResult<Country>>> Handle(
        GetCountriesQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.PaginationRequest;

        var cacheKey =
            $"{CacheKeys.Countries}:" +
            $"page:{pagination.PageNumber}:" +
            $"size:{pagination.PageSize}";

        var cachedResponse = await cacheService.GetAsync<PaginatedResult<Country>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResult<Country>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Country),
                HttpStatusCode.OK
            );
        }

        var result = await countryRepository.GetAllAsync(pagination, cancellationToken);

        var response = new PaginatedResult<Country>
        {
            Items = mapper.Map<IReadOnlyList<Country>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<PaginatedResult<Country>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Country),
            HttpStatusCode.OK
        );
    }
}