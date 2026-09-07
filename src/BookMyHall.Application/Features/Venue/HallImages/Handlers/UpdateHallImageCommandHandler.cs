using System.Net;

using AutoMapper;
using MediatR;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence;

using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;

using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Messaging;
using BookMyHall.Contracts.Venue;

using BookMyHall.Domain.Venue;

using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateHallImageCommandHandler(
    IHallImageRepository hallImageRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IR2StorageService r2StorageService,
    IMessagePublisher messagePublisher)
    : IRequestHandler<
        UpdateHallImageCommand,
        ApiResponse<HallImageDto>>
{
    private static readonly TimeSpan PreSignedUrlExpiration =
        TimeSpan.FromMinutes(30);

    public async Task<ApiResponse<HallImageDto>> Handle(
        UpdateHallImageCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // 1. GET EXISTING HALL IMAGE
        // =========================================================

        var hallImage =
            await hallImageRepository.GetByIdAsync(
                request.HallImageId,
                cancellationToken);

        if (hallImage is null)
        {
            return ApiResponse<HallImageDto>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallImage),
                HttpStatusCode.NotFound);
        }

        // =========================================================
        // 2. STORE HALL ID
        // =========================================================
        //
        // The HallId is required for hall-specific cache
        // invalidation.
        //
        // =========================================================

        var hallId =
            hallImage.HallId;

        var hallImageId =
            hallImage.HallImageId;

        // =========================================================
        // 3. STORE OLD R2 OBJECT KEYS
        // =========================================================
        //
        // These are persistent R2 object keys.
        //
        // They are NOT pre-signed URLs.
        //
        // We need them for cleanup after the new image is
        // successfully persisted.
        //
        // =========================================================

        var oldImageKey =
            hallImage.ImageUrl;

        var oldThumbnailKey =
            hallImage.ThumbnailUrl;

        // =========================================================
        // 4. CHECK WHETHER IMAGE IS BEING REPLACED
        // =========================================================

        var imageReplaced =
            request.ImageStream is not null;

        // =========================================================
        // 5. VALIDATE REPLACEMENT FILE
        // =========================================================

        if (imageReplaced)
        {
            // -----------------------------------------------------
            // Validate file name
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(
                    request.FileName))
            {
                return ApiResponse<HallImageDto>.FailureResponse(
                    "File name is required when replacing the image.",
                    HttpStatusCode.BadRequest);
            }

            // -----------------------------------------------------
            // Validate content type
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(
                    request.ContentType))
            {
                return ApiResponse<HallImageDto>.FailureResponse(
                    "Content type is required when replacing the image.",
                    HttpStatusCode.BadRequest);
            }
        }

        // =========================================================
        // 6. UPDATE METADATA / REPLACE IMAGE
        // =========================================================
        //
        // If a new image is supplied:
        //
        //     New original image is uploaded to R2.
        //     ThumbnailUrl is reset to null.
        //     RabbitMQ will generate a new thumbnail.
        //
        // If no new image is supplied:
        //
        //     Only metadata is updated.
        //
        // =========================================================

        if (imageReplaced)
        {
            await ReplaceImageAsync(
                hallImage,
                request,
                cancellationToken);
        }
        else
        {
            // -----------------------------------------------------
            // No image replacement.
            //
            // Update only metadata.
            // -----------------------------------------------------

            hallImage.SetCoverImage(
                request.IsCoverImage);

            hallImage.UpdateDisplayOrder(
                request.DisplayOrder);

            hallImage.SetActive(
                request.IsActive);
        }

        // =========================================================
        // 7. UPDATE DATABASE
        // =========================================================

        await hallImageRepository.UpdateAsync(
            hallImage,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // =========================================================
        // 8. INVALIDATE CACHE
        // =========================================================
        //
        // IMPORTANT:
        //
        // Database changes have now been successfully persisted.
        //
        // Therefore invalidate cache immediately.
        //
        // We DO NOT cache the response because the response may
        // contain temporary R2 pre-signed URLs.
        //
        // =========================================================

        await InvalidateHallImageCachesAsync(
            hallId,
            hallImageId,
            cancellationToken);

        // =========================================================
        // 9. PUBLISH THUMBNAIL GENERATION MESSAGE
        // =========================================================
        //
        // Only required when the original image was replaced.
        //
        // RabbitMQ will asynchronously generate the thumbnail.
        //
        // =========================================================

        if (imageReplaced)
        {
            var message =
                new HallImageUploadedMessage(
                    HallImageId:
                        hallImage.HallImageId,

                    HallId:
                        hallImage.HallId,

                    ObjectKey:
                        hallImage.ImageUrl);

            await messagePublisher.PublishAsync(
                message,
                cancellationToken);
        }

        // =========================================================
        // 10. DELETE OLD R2 ORIGINAL IMAGE
        // =========================================================
        //
        // Only delete the old original image when the object key
        // actually changed.
        //
        // =========================================================

        if (imageReplaced &&
            !string.IsNullOrWhiteSpace(oldImageKey) &&
            !string.Equals(
                oldImageKey,
                hallImage.ImageUrl,
                StringComparison.OrdinalIgnoreCase))
        {
            await DeleteR2ObjectSafelyAsync(
                oldImageKey);
        }

        // =========================================================
        // 11. DELETE OLD R2 THUMBNAIL
        // =========================================================
        //
        // The old thumbnail is no longer referenced after the
        // original image is replaced.
        //
        // =========================================================

        if (imageReplaced &&
            !string.IsNullOrWhiteSpace(oldThumbnailKey))
        {
            await DeleteR2ObjectSafelyAsync(
                oldThumbnailKey);
        }

        // =========================================================
        // 12. MAP ENTITY → DTO
        // =========================================================

        var response =
            mapper.Map<HallImageDto>(
                hallImage);

        // =========================================================
        // 13. VALIDATE ORIGINAL IMAGE OBJECT KEY
        // =========================================================

        if (string.IsNullOrWhiteSpace(
                hallImage.ImageUrl))
        {
            return ApiResponse<HallImageDto>.FailureResponse(
                "Hall image object key is missing.",
                HttpStatusCode.InternalServerError);
        }

        // =========================================================
        // 14. GENERATE FRESH ORIGINAL PRE-SIGNED URL
        // =========================================================
        //
        // IMPORTANT:
        //
        // This URL is generated AFTER cache invalidation.
        //
        // It is never stored in cache.
        //
        // =========================================================

        var imageUrl =
            await r2StorageService.GetPreSignedUrlAsync(
                hallImage.ImageUrl,
                PreSignedUrlExpiration,
                cancellationToken);

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return ApiResponse<HallImageDto>.FailureResponse(
                "Unable to generate pre-signed URL for the Hall image.",
                HttpStatusCode.InternalServerError);
        }

        response.ImageUrl =
            imageUrl;

        // =========================================================
        // 15. GENERATE FRESH THUMBNAIL PRE-SIGNED URL
        // =========================================================
        //
        // After image replacement:
        //
        //     ThumbnailUrl = null
        //
        // RabbitMQ generates the thumbnail asynchronously.
        //
        // Therefore null is expected immediately after replacing
        // an image.
        //
        // =========================================================

        if (!string.IsNullOrWhiteSpace(
                hallImage.ThumbnailUrl))
        {
            var thumbnailUrl =
                await r2StorageService.GetPreSignedUrlAsync(
                    hallImage.ThumbnailUrl,
                    PreSignedUrlExpiration,
                    cancellationToken);

            response.ThumbnailUrl =
                string.IsNullOrWhiteSpace(
                    thumbnailUrl)
                    ? null
                    : thumbnailUrl;
        }
        else
        {
            response.ThumbnailUrl =
                null;
        }

        // =========================================================
        // 16. DO NOT CACHE RESPONSE
        // =========================================================
        //
        // IMPORTANT:
        //
        // response.ImageUrl and response.ThumbnailUrl can contain
        // temporary R2 pre-signed URLs.
        //
        // NEVER store this response in cache.
        //
        // Cache contains only persistent image metadata.
        //
        // =========================================================

        // =========================================================
        // 17. RETURN SUCCESS
        // =========================================================

        return ApiResponse<HallImageDto>.SuccessResponse(
            response,

            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.HallImage),

            HttpStatusCode.OK);
    }

    // =============================================================
    // REPLACE IMAGE
    // =============================================================

    private async Task ReplaceImageAsync(
        HallImage hallImage,
        UpdateHallImageCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // 1. VALIDATE STREAM
        // =========================================================

        ArgumentNullException.ThrowIfNull(
            request.ImageStream);

        // =========================================================
        // 2. GET FILE EXTENSION
        // =========================================================

        var extension =
            Path.GetExtension(
                request.FileName!)
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(
                extension))
        {
            throw new InvalidOperationException(
                "Unable to determine image file extension.");
        }

        // =========================================================
        // 3. GENERATE NEW R2 OBJECT KEY
        // =========================================================
        //
        // Same HallImageId is retained.
        //
        // Example:
        //
        // halls/{hallId}/{hallImageId}.jpg
        //
        // =========================================================

        var newImageKey =
            $"halls/{hallImage.HallId}/" +
            $"{hallImage.HallImageId}{extension}";

        // =========================================================
        // 4. RESET STREAM POSITION
        // =========================================================

        if (request.ImageStream.CanSeek)
        {
            request.ImageStream.Position = 0;
        }

        // =========================================================
        // 5. UPLOAD NEW ORIGINAL IMAGE
        // =========================================================

        try
        {
            await r2StorageService.UploadAsync(
                request.ImageStream,
                newImageKey,
                request.ContentType!,
                cancellationToken);
        }
        catch
        {
            // -----------------------------------------------------
            // Upload failed.
            //
            // Try to remove partially uploaded object.
            // -----------------------------------------------------

            try
            {
                await r2StorageService.DeleteAsync(
                    newImageKey,
                    CancellationToken.None);
            }
            catch
            {
                // Do not hide original exception.
            }

            throw;
        }

        // =========================================================
        // 6. UPDATE ENTITY WITH NEW OBJECT KEY
        // =========================================================
        //
        // ThumbnailUrl is intentionally reset.
        //
        // RabbitMQ will generate the new thumbnail.
        //
        // =========================================================

        hallImage.Update(
            imageUrl:
                newImageKey,

            thumbnailUrl:
                null,

            displayOrder:
                request.DisplayOrder,

            isCoverImage:
                request.IsCoverImage);

        hallImage.SetActive(
            request.IsActive);
    }

    // =============================================================
    // CACHE INVALIDATION
    // =============================================================

    private async Task InvalidateHallImageCachesAsync(
        Guid hallId,
        Guid hallImageId,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // 1. CLEAR INDIVIDUAL IMAGE CACHE
        // =========================================================
        //
        // Key:
        //
        // hallimage:{hallImageId}
        //
        // =========================================================

        await cacheService.RemoveAsync(
            HallImageCacheKeyBuilder.BuildImageKey(
                hallImageId),
            cancellationToken);

        // =========================================================
        // 2. CLEAR HALL-SPECIFIC PAGINATED CACHE
        // =========================================================
        //
        // Prefix:
        //
        // hallimages:page:{hallId}:
        //
        // This clears all pagination combinations for THIS hall:
        //
        // page 1 / page 2 / page 3
        // different page sizes
        // different sorting
        // different searches
        //
        // It does NOT clear image caches belonging to other halls.
        //
        // =========================================================

        await cacheService.RemoveByPrefixAsync(
            HallImageCacheKeyBuilder.BuildHallPaginatedPrefix(
                hallId),
            cancellationToken);

        // =========================================================
        // 3. CLEAR HALL COVER IMAGE CACHE
        // =========================================================
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
    }

    // =============================================================
    // SAFE R2 DELETE
    // =============================================================

    private async Task DeleteR2ObjectSafelyAsync(
        string objectKey)
    {
        try
        {
            await r2StorageService.DeleteAsync(
                objectKey,
                CancellationToken.None);
        }
        catch
        {
            // =====================================================
            // IMPORTANT
            // =====================================================
            //
            // Database has already been updated.
            //
            // R2 cleanup failure should not make the update
            // operation fail.
            //
            // =====================================================
        }
    }
}