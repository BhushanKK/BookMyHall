using System.Net;

using MediatR;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteHallBlockCommandHandler(
    IHallBlockRepository hallBlockRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        DeleteHallBlockCommand,
        ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteHallBlockCommand request,
        CancellationToken cancellationToken)
    {
        // =====================================================
        // GET EXISTING RECORD
        // =====================================================

        var hallBlock =
            await hallBlockRepository.GetByIdAsync(
                request.HallBlockId,
                cancellationToken);

        if (hallBlock is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallBlock),
                HttpStatusCode.NotFound);
        }


        // =====================================================
        // SOFT DELETE
        // =====================================================

        hallBlock.IsDeleted = true;


        // =====================================================
        // SAVE
        // =====================================================

        await hallBlockRepository.UpdateAsync(
            hallBlock,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);


        // =====================================================
        // CACHE INVALIDATION
        //
        // 1. Remove individual HallBlock cache.
        // 2. Remove all paginated caches because:
        //
        //    - total count changes
        //    - pagination changes
        //    - Hall filtered results change
        //    - search results change
        // =====================================================

        await cacheService.RemoveAsync(
            HallBlockCacheKeyBuilder.BuildByIdKey(
                request.HallBlockId),
            cancellationToken);

        await cacheService.RemoveByPrefixAsync(
            HallBlockCacheKeyBuilder.BuildPaginatedPrefix(),
            cancellationToken);


        // =====================================================
        // RESPONSE
        // =====================================================

        return ApiResponse<bool>.SuccessResponse(
            true,
            messageHelper.DeletedEntity(
                ResourceNames.Entities,
                EntityKeys.HallBlock),
            HttpStatusCode.OK);
    }
}