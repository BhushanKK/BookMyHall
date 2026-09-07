using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence;

using System.Net;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteHallImageCommandHandler(
    IHallImageRepository hallImageRepository,
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IUnitOfWork unitOfWork)
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
        // 4. KEEP HALL ID
        // =========================================================
        //
        // We need HallId for invalidating the cover-image cache.
        //
        // =========================================================

        var hallId = hallImage.HallId;

        // =========================================================
        // 5. SOFT DELETE IMAGE
        // =========================================================
        //
        // We do not physically delete the database record.
        //
        // The image is marked as deleted.
        //
        // A deleted image cannot remain the cover image.
        //
        // =========================================================

        hallImage.IsDeleted = true;
        hallImage.IsCoverImage = false;

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
        // IMPORTANT:
        //
        // Cache must NOT be invalidated before the database
        // transaction is persisted.
        //
        // =========================================================

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // =========================================================
        // 8. CLEAR SINGLE IMAGE CACHE
        // =========================================================
        //
        // Cache key:
        //
        // hallimage:{hallImageId}
        //
        // =========================================================

        await cacheService.RemoveAsync(
            $"{CacheKeys.HallImage}:{request.HallImageId}",
            cancellationToken);

        // =========================================================
        // 9. CLEAR PAGINATED HALL IMAGE CACHE
        // =========================================================
        //
        // IMPORTANT:
        //
        // CacheKeys.HallImagesPaged already ends with ":".
        //
        // Value:
        //
        // hallimages:page:
        //
        // Therefore DO NOT add another ":".
        //
        // WRONG:
        //
        // $"{CacheKeys.HallImagesPaged}:"
        //
        // Result:
        //
        // hallimages:page::
        //
        // CORRECT:
        //
        // CacheKeys.HallImagesPaged
        //
        // =========================================================

        await cacheService.RemoveByPrefixAsync(
            CacheKeys.HallImagesPaged,
            cancellationToken);

        // =========================================================
        // 10. CLEAR HALL COVER IMAGE CACHE
        // =========================================================
        //
        // The deleted image may have been the cover image.
        //
        // Cache key:
        //
        // HallCoverImage:{hallId}
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