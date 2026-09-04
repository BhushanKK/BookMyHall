using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallQueryHandler(IHallRepository hallRepository,
    IMessageHelper messageHelper,IMapper mapper,ICacheService cacheService)
    : IRequestHandler<GetHallQuery, ApiResponse<PaginatedResult<HallListDto>>>
{
    public async Task<ApiResponse<PaginatedResult<HallListDto>>> Handle(GetHallQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;

          var cacheKey = CacheKeyBuilder.BuildPaginatedKey<HallListDto>(
            CacheKeys.Hall,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResult<HallListDto>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResult<HallListDto>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Hall),
                HttpStatusCode.OK
            );
        }
        var result = await hallRepository.GetAllAsync(request.paginationRequest, cancellationToken);

        var response = new PaginatedResult<HallListDto>
        {
            Items = mapper.Map<IReadOnlyList<HallListDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResult<HallListDto>>.SuccessResponse
        (response,messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Hall),
            HttpStatusCode.OK
        );
    }
}