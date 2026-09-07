using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallBlocksQueryHandler(
    IHallBlockRepository hallBlockRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        GetHallBlocksQuery,
        ApiResponse<PaginatedResponse<HallBlockDto>>>
{
    public async Task<
        ApiResponse<PaginatedResponse<HallBlockDto>>>
        Handle(
            GetHallBlocksQuery request,
            CancellationToken cancellationToken)
    {
        // =====================================================
        // PAGINATION
        // =====================================================

        var pagination =
            request.paginationRequest;


        // =====================================================
        // HALL FILTER
        // =====================================================

        var hallId =
            request.HallId;


        // =====================================================
        // CACHE KEY
        //
        // HallId MUST be part of the cache key.
        //
        // Example:
        //
        // hallblocks:paged:
        // hall:none:
        // page:1:
        // size:10:
        // ...
        //
        // vs.
        //
        // hallblocks:paged:
        // hall:{hallId}:
        // page:1:
        // size:10:
        // ...
        // =====================================================

        var cacheKey =
            HallBlockCacheKeyBuilder.BuildPaginatedKey(
                hallId,
                pagination.PageNumber,
                pagination.PageSize,
                pagination.SearchText,
                pagination.SortBy,
                pagination.SortDescending);


        // =====================================================
        // CHECK CACHE
        // =====================================================

        var cachedResponse =
            await cacheService.GetAsync<
                PaginatedResponse<HallBlockDto>>(
                cacheKey,
                cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<
                PaginatedResponse<HallBlockDto>>.SuccessResponse(
                cachedResponse,
                messageHelper.RetrievedEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallBlock),
                HttpStatusCode.OK);
        }


        // =====================================================
        // DATABASE
        //
        // HallId is passed to repository.
        // =====================================================

        var pagedResult =
            await hallBlockRepository.GetAllAsync(
                pagination,
                hallId,
                cancellationToken);


        // =====================================================
        // MAP RESULT
        // =====================================================

        var response =
            new PaginatedResponse<HallBlockDto>
            {
                Items =
                    mapper.Map<IReadOnlyList<HallBlockDto>>(
                        pagedResult.Items),

                PageNumber =
                    pagedResult.PageNumber,

                PageSize =
                    pagedResult.PageSize,

                TotalRecords =
                    pagedResult.TotalCount
            };


        // =====================================================
        // CACHE
        // =====================================================

        await cacheService.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(30),
            cancellationToken);


        // =====================================================
        // RESPONSE
        // =====================================================

        return ApiResponse<
            PaginatedResponse<HallBlockDto>>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.HallBlock),
            HttpStatusCode.OK);
    }
}