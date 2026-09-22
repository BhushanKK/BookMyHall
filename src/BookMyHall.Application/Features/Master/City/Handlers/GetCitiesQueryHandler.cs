using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCitiesQueryHandler(
    ICityRepository cityRepository, IMessageHelper messageHelper,
    IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetCitiesQuery, ApiResponse<PaginatedResponse<City>>>
{
    public async Task<ApiResponse<PaginatedResponse<City>>> Handle(
        GetCitiesQuery request, CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;

        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<City>(
            CacheKeys.CitiesPaged, pagination.PageNumber,
            pagination.PageSize, pagination.SearchText,
            pagination.SortBy, pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<City>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<City>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.City),
                HttpStatusCode.OK
            );
        }

        var result = await cityRepository.GetAllAsync(pagination, cancellationToken);

        var response = new PaginatedResponse<City>
        {
            Items = mapper.Map<IReadOnlyList<City>>(result.Items),
            TotalRecords = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<PaginatedResponse<City>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.City),
            HttpStatusCode.OK
        );
    }
}