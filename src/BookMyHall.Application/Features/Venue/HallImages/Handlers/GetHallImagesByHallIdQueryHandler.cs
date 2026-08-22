using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallImagesByHallIdQueryHandler(
    IHallImageRepository hallImageRepository,
    IMapper mapper,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<
        GetHallImagesByHallIdQuery,
        ApiResponse<PaginatedResult<HallImageDto>>>
{
    public async Task<ApiResponse<PaginatedResult<HallImageDto>>> Handle(
        GetHallImagesByHallIdQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.Pagination;
         var cacheKey = CacheKeyBuilder.BuildPaginatedKey<HallImage>(
            CacheKeys.HallImage,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResult =await cacheService.GetAsync<PaginatedResult<HallImageDto>>(cacheKey,cancellationToken);

        if (cachedResult is not null)
        {
            return ApiResponse<PaginatedResult<HallImageDto>>.SuccessResponse(cachedResult,
                messageHelper.RetrievedEntity( ResourceNames.Entities,
                    EntityKeys.HallImage),HttpStatusCode.OK);
        }

        var result = await hallImageRepository.GetByHallIdAsync(
            request.HallId,
            request.Pagination,
            cancellationToken);

        if (result.Items is null || result.Items.Count == 0)
        {
            return ApiResponse<PaginatedResult<HallImageDto>>.FailureResponse
            (
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallImage),
                HttpStatusCode.NotFound
            );
        }

        var mappedResult = new PaginatedResult<HallImageDto>
        {
            Items = mapper.Map<IReadOnlyList<HallImageDto>>(
                result.Items),

            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        
        await cacheService.SetAsync(cacheKey, mappedResult,TimeSpan.FromMinutes(30),cancellationToken);

        return ApiResponse<PaginatedResult<HallImageDto>>.SuccessResponse
        (
            mappedResult,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.HallImage),
            HttpStatusCode.OK
        );
    }
}