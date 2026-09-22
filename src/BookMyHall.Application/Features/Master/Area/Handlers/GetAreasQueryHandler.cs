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

public sealed class GetAreasQueryHandler(IAreaRepository areaRepository,
    IMessageHelper messageHelper,IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetAreasQuery, ApiResponse<PaginatedResponse<Area>>>
{
    public async Task<ApiResponse<PaginatedResponse<Area>>> Handle(GetAreasQuery request, CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<Area>(
            CacheKeys.AreasPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<Area>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<Area>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Area),
                HttpStatusCode.OK
            );
        }
        var result = await areaRepository.GetAllAsync(request.paginationRequest, cancellationToken);
        var response = new PaginatedResponse<Area>
        {
            Items = mapper.Map<IReadOnlyList<Area>>(result.Items),
            TotalRecords = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<Area>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Area), HttpStatusCode.OK);
    }
}