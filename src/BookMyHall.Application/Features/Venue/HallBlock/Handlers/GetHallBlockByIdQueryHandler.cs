using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallBlockByIdQueryHandler(
    IHallBlockRepository hallBlockRepository,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        GetHallBlockByIdQuery,
        ApiResponse<HallBlock>>
{
    public async Task<ApiResponse<HallBlock>> Handle(
        GetHallBlockByIdQuery request,
        CancellationToken cancellationToken)
    {
        // =====================================================
        // CACHE KEY
        // =====================================================

        var cacheKey =
            HallBlockCacheKeyBuilder.BuildByIdKey(
                request.HallBlockId);


        // =====================================================
        // CHECK CACHE
        // =====================================================

        var cachedHallBlock =
            await cacheService.GetAsync<HallBlock>(
                cacheKey,
                cancellationToken);

        if (cachedHallBlock is not null)
        {
            return ApiResponse<HallBlock>.SuccessResponse(
                cachedHallBlock,
                messageHelper.RetrievedEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallBlock),
                HttpStatusCode.OK);
        }


        // =====================================================
        // DATABASE
        // =====================================================

        var hallBlock =
            await hallBlockRepository.GetByIdAsync(
                request.HallBlockId,
                cancellationToken);

        if (hallBlock is null)
        {
            return ApiResponse<HallBlock>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallBlock),
                HttpStatusCode.NotFound);
        }


        // =====================================================
        // CACHE
        // =====================================================

        await cacheService.SetAsync(
            cacheKey,
            hallBlock,
            TimeSpan.FromMinutes(30),
            cancellationToken);


        // =====================================================
        // RESPONSE
        // =====================================================

        return ApiResponse<HallBlock>.SuccessResponse(
            hallBlock,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.HallBlock),
            HttpStatusCode.OK);
    }
}