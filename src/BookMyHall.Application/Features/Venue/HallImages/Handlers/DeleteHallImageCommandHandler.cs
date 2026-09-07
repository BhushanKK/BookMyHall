using System.Net;

using MediatR;

using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteHallImageCommandHandler(
    IHallImageRepository hallImageRepository,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<DeleteHallImageCommand, ApiResponse<bool>>
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

        if (!hallImage.IsActive)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallImage),
                HttpStatusCode.NotFound);
        }

        // =========================================================
        // 4. KEEP HALL ID BEFORE UPDATE
        // =========================================================
        //
        // We need HallId for hall-specific cache invalidation.
        //
        // =========================================================

        var hallId =
            hallImage.HallId;

        // =========================================================
        // 5. SOFT DELETE IMAGE
        // =========================================================

        hallImage.IsDeleted = true;

        // A deleted image cannot remain the cover image.
        hallImage.IsCoverImage = false;

        // =========================================================
        // 6. UPDATE DATABASE
        // =========================================================

        await hallImageRepository.UpdateAsync(
            hallImage,
            cancellationToken);

        // =========================================================
        // 7. SAVE / UNIT OF WORK
        // =========================================================
        //
        // IMPORTANT:
        //
        // If your repository UpdateAsync does not automatically
        // persist changes, SaveChangesAsync must be called here.
        //
        // If your architecture uses a UnitOfWork elsewhere,
        // inject IUnitOfWork and call SaveChangesAsync here.
        //
        // =========================================================

        // =========================================================
        // 8. CLEAR SINGLE IMAGE CACHE
        // =========================================================

        await cacheService.RemoveAsync(
            $"{CacheKeys.HallImage}:{request.HallImageId}",
            cancellationToken);

        // =========================================================
        // 9. CLEAR PAGINATED HALL IMAGE CACHE
        // =========================================================

        await cacheService.RemoveByPrefixAsync(
            $"{CacheKeys.HallImagesPaged}:",
            cancellationToken);

        // =========================================================
        // 10. CLEAR HALL COVER IMAGE CACHE
        // =========================================================
        //
        // This is important because the deleted image may have
        // previously been the cover image.
        //
        // =========================================================

        await cacheService.RemoveAsync(
            $"{CacheKeys.HallCoverImage}:{hallId}",
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