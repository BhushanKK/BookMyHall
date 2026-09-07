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
        // 2. STORE OLD OBJECT KEYS
        // =========================================================
        //
        // We need these before modifying the entity.
        //
        // ImageUrl       = R2 object key
        // ThumbnailUrl   = R2 thumbnail object key
        //
        // These are NOT signed URLs.
        //
        // =========================================================

        var oldImageKey =
            hallImage.ImageUrl;

        var oldThumbnailKey =
            hallImage.ThumbnailUrl;

        // =========================================================
        // 3. UPDATE METADATA
        // =========================================================

        hallImage.SetCoverImage(
            request.IsCoverImage);

        hallImage.UpdateDisplayOrder(
            request.DisplayOrder);

        hallImage.SetActive(
            request.IsActive);

        // =========================================================
        // 4. CHECK WHETHER IMAGE IS BEING REPLACED
        // =========================================================

        var imageReplaced =
            request.ImageStream is not null;

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

            // -----------------------------------------------------
            // Replace R2 image
            // -----------------------------------------------------

            await ReplaceImageAsync(
                hallImage,
                request,
                cancellationToken);
        }

        // =========================================================
        // 5. UPDATE DATABASE
        // =========================================================

        await hallImageRepository.UpdateAsync(
            hallImage,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // =========================================================
        // 6. PUBLISH THUMBNAIL GENERATION MESSAGE
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

            // -----------------------------------------------------
            // Delete old ORIGINAL image if the object key changed
            // -----------------------------------------------------

            if (!string.IsNullOrWhiteSpace(
                    oldImageKey)
                &&
                !string.Equals(
                    oldImageKey,
                    hallImage.ImageUrl,
                    StringComparison.OrdinalIgnoreCase))
            {
                await DeleteR2ObjectSafelyAsync(
                    oldImageKey);
            }

            // -----------------------------------------------------
            // Delete old THUMBNAIL
            // -----------------------------------------------------
            //
            // The old thumbnail is no longer referenced after
            // replacing the original image.
            //
            // -----------------------------------------------------

            if (!string.IsNullOrWhiteSpace(
                    oldThumbnailKey))
            {
                await DeleteR2ObjectSafelyAsync(
                    oldThumbnailKey);
            }
        }

        // =========================================================
        // 7. INVALIDATE ALL HALL IMAGE CACHES
        // =========================================================

        await InvalidateHallImageCachesAsync(
            hallImage.HallId,
            hallImage.HallImageId,
            cancellationToken);

        // =========================================================
        // 8. MAP ENTITY → DTO
        // =========================================================

        var response =
            mapper.Map<HallImageDto>(
                hallImage);

        // =========================================================
        // 9. GENERATE FRESH ORIGINAL PRE-SIGNED URL
        // =========================================================

        if (string.IsNullOrWhiteSpace(
                hallImage.ImageUrl))
        {
            return ApiResponse<HallImageDto>.FailureResponse(
                "Hall image object key is missing.",
                HttpStatusCode.InternalServerError);
        }

        var imageUrl =
            await r2StorageService.GetPreSignedUrlAsync(
                hallImage.ImageUrl,
                PreSignedUrlExpiration,
                cancellationToken);

        if (string.IsNullOrWhiteSpace(
                imageUrl))
        {
            return ApiResponse<HallImageDto>.FailureResponse(
                "Unable to generate pre-signed URL for the Hall image.",
                HttpStatusCode.InternalServerError);
        }

        response.ImageUrl =
            imageUrl;

        // =========================================================
        // 10. GENERATE FRESH THUMBNAIL PRE-SIGNED URL
        // =========================================================
        //
        // When image is replaced:
        //
        //     ThumbnailUrl = null
        //
        // RabbitMQ will generate the thumbnail asynchronously.
        //
        // Therefore it is perfectly valid for this response to
        // have ThumbnailUrl = null immediately after update.
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
            response.ThumbnailUrl = null;
        }

        // =========================================================
        // 11. IMPORTANT:
        //     DO NOT CACHE RESPONSE
        // =========================================================
        //
        // response contains temporary R2 pre-signed URLs.
        //
        // NEVER do:
        //
        // cacheService.SetAsync(
        //     cacheKey,
        //     response,
        //     ...);
        //
        // The cache invalidation above is enough.
        //
        // =========================================================

        // =========================================================
        // 12. RETURN RESPONSE
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
        ArgumentNullException.ThrowIfNull(
            request.ImageStream);

        // =========================================================
        // 1. GET FILE EXTENSION
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
        // 2. GENERATE NEW R2 OBJECT KEY
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
        // 3. RESET STREAM
        // =========================================================

        if (request.ImageStream.CanSeek)
        {
            request.ImageStream.Position = 0;
        }

        // =========================================================
        // 4. UPLOAD NEW ORIGINAL IMAGE
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
            // Try to remove partially uploaded R2 object.
            // -----------------------------------------------------

            try
            {
                await r2StorageService.DeleteAsync(
                    newImageKey,
                    CancellationToken.None);
            }
            catch
            {
                // Do not hide the original exception.
            }

            throw;
        }

        // =========================================================
        // 5. UPDATE ENTITY
        // =========================================================
        //
        // ThumbnailUrl is deliberately reset.
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
        // ---------------------------------------------------------
        // 1. SINGLE IMAGE CACHE
        // ---------------------------------------------------------

        await cacheService.RemoveAsync(
            $"{CacheKeys.HallImage}:{hallImageId}",
            cancellationToken);

        // ---------------------------------------------------------
        // 2. COVER IMAGE CACHE
        // ---------------------------------------------------------

        await cacheService.RemoveAsync(
            $"{CacheKeys.HallCoverImage}:{hallId}",
            cancellationToken);

        // ---------------------------------------------------------
        // 3. PAGINATED IMAGE CACHE
        // ---------------------------------------------------------

        await cacheService.RemoveByPrefixAsync(
            $"{CacheKeys.HallImagesPaged}:",
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
            // -----------------------------------------------------
            // Cleanup failure must not make the update operation
            // fail after the database has already been updated.
            // -----------------------------------------------------
        }
    }
}