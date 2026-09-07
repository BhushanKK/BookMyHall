using System.Net;

using MediatR;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;

using BookMyHall.Application.Common.Interfaces.Repositories.Venue;

using BookMyHall.Contracts.Common;

using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteHallImageCommandHandler(
    IHallImageRepository hallImageRepository,
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        DeleteHallImageCommand,
        ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteHallImageCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // 1. GET HALL IMAGE
        // =========================================================

        var hallImage =
            await hallImageRepository.GetByIdAsync(
                request.HallImageId,
                cancellationToken);

        // =========================================================
        // 2. CHECK IMAGE EXISTS
        // =========================================================

        if (hallImage is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallImage),
                HttpStatusCode.NotFound);
        }

        // =========================================================
        // 3. CHECK IMAGE IS ACTIVE
        // =========================================================
        //
        // Existing behavior is preserved.
        //
        // An inactive image cannot be deleted again.
        //
        // =========================================================

        if (!hallImage.IsActive)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallImage),
                HttpStatusCode.NotFound);
        }

        // =========================================================
        // 4. KEEP HALL ID
        // =========================================================
        //
        // We need HallId because:
        //
        // 1. Paginated cache is hall-specific.
        // 2. Cover-image cache is hall-specific.
        //
        // =========================================================

        var hallId =
            hallImage.HallId;

        // =========================================================
        // 5. SOFT DELETE IMAGE
        // =========================================================
        //
        // We do not physically remove the database record.
        //
        // The image is marked as deleted.
        //
        // Also remove its cover-image status.
        //
        // =========================================================

        hallImage.IsDeleted =
            true;

        hallImage.IsCoverImage =
            false;

        // =========================================================
        // 6. UPDATE ENTITY
        // =========================================================

        await hallImageRepository.UpdateAsync(
            hallImage,
            cancellationToken);

        // =========================================================
        // 7. SAVE DATABASE CHANGES
        // =========================================================
        //
        // Cache invalidation happens ONLY after the database
        // changes have been successfully persisted.
        //
        // =========================================================

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // =========================================================
        // 8. CLEAR INDIVIDUAL IMAGE CACHE
        // =========================================================
        //
        // Key:
        //
        // hallimage:{hallImageId}
        //
        // Example:
        //
        // hallimage:8f5d...
        //
        // =========================================================

        await cacheService.RemoveAsync(
            HallImageCacheKeyBuilder.BuildImageKey(
                request.HallImageId),
            cancellationToken);

        // =========================================================
        // 9. CLEAR HALL-SPECIFIC PAGINATED IMAGE CACHE
        // =========================================================
        //
        // IMPORTANT:
        //
        // Do NOT use:
        //
        // CacheKeys.HallImagesPaged
        //
        // directly here because that would invalidate the
        // paginated image cache for EVERY hall.
        //
        // Instead use the hall-specific prefix:
        //
        // hallimages:page:{hallId}:
        //
        // This clears:
        //
        // hallimages:page:{hallId}:page:1:size:10:...
        // hallimages:page:{hallId}:page:2:size:10:...
        // hallimages:page:{hallId}:page:3:size:20:...
        //
        // while leaving other halls' caches untouched.
        //
        // =========================================================

        await cacheService.RemoveByPrefixAsync(
            HallImageCacheKeyBuilder.BuildHallPaginatedPrefix(
                hallId),
            cancellationToken);

        // =========================================================
        // 10. CLEAR HALL COVER IMAGE CACHE
        // =========================================================
        //
        // The deleted image may have been the hall's cover image.
        //
        // Key:
        //
        // HallCoverImage:{hallId}
        //
        // =========================================================

        await cacheService.RemoveAsync(
            HallImageCacheKeyBuilder.BuildCoverImageKey(
                hallId),
            cancellationToken);

        // =========================================================
        // 11. RETURN SUCCESS
        // =========================================================

        return ApiResponse<bool>.SuccessResponse(
            true,

            messageHelper.DeletedEntity(
                ResourceNames.Entities,
                EntityKeys.HallImage),

            HttpStatusCode.OK);
    }
}