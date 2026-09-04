using MediatR;
using System.Net;
using AutoMapper;
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
    : IRequestHandler<UpdateHallImageCommand, ApiResponse<HallImageDto>>
{
    public async Task<ApiResponse<HallImageDto>> Handle(
        UpdateHallImageCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Get existing Hall Image
        // ---------------------------------------------------------

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

        // ---------------------------------------------------------
        // 2. Store old image key
        // ---------------------------------------------------------

        var oldImageKey = hallImage.ImageUrl;

        // ---------------------------------------------------------
        // 3. Update metadata
        // ---------------------------------------------------------

        hallImage.SetCoverImage(
            request.IsCoverImage);

        hallImage.UpdateDisplayOrder(
            request.DisplayOrder);

        hallImage.SetActive(
            request.IsActive);

        // ---------------------------------------------------------
        // 4. Replace image if a new file is supplied
        // ---------------------------------------------------------

        var imageReplaced =
            request.ImageStream is not null;

        if (imageReplaced)
        {
            if (string.IsNullOrWhiteSpace(request.FileName))
            {
                return ApiResponse<HallImageDto>.FailureResponse(
                    "File name is required when replacing the image.",
                    HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(request.ContentType))
            {
                return ApiResponse<HallImageDto>.FailureResponse(
                    "Content type is required when replacing the image.",
                    HttpStatusCode.BadRequest);
            }

            await ReplaceImageAsync(
                hallImage,
                request,
                cancellationToken);
        }

        // ---------------------------------------------------------
        // 5. Save database
        // ---------------------------------------------------------

        await hallImageRepository.UpdateAsync(
            hallImage,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // ---------------------------------------------------------
        // 6. Publish thumbnail generation message
        // ---------------------------------------------------------

        if (imageReplaced)
        {
            var message =
                new HallImageUploadedMessage(
                    HallImageId: hallImage.HallImageId,
                    HallId: hallImage.HallId,
                    ObjectKey: hallImage.ImageUrl);

            await messagePublisher.PublishAsync(
                message,
                cancellationToken);

            // -----------------------------------------------------
            // 7. Delete old original after successful publish
            // -----------------------------------------------------

            if (!string.IsNullOrWhiteSpace(oldImageKey) &&
                !string.Equals(
                    oldImageKey,
                    hallImage.ImageUrl,
                    StringComparison.OrdinalIgnoreCase))
            {
                await DeleteR2ObjectSafelyAsync(
                    oldImageKey);
            }
        }

        // ---------------------------------------------------------
        // 8. Clear cache
        // ---------------------------------------------------------

        await cacheService.RemoveAsync(
            $"{CacheKeys.HallImage}:{request.HallImageId}",
            cancellationToken);

        await cacheService.RemoveAsync(
            $"{CacheKeys.HallCoverImage}:{hallImage.HallId}",
            cancellationToken);

        await cacheService.RemoveByPrefixAsync(
            $"{CacheKeys.HallImagesPaged}:",
            cancellationToken);

        // ---------------------------------------------------------
        // 9. Return response
        // ---------------------------------------------------------

        return ApiResponse<HallImageDto>.SuccessResponse(
            mapper.Map<HallImageDto>(hallImage),
            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.HallImage),
            HttpStatusCode.OK);
    }

    private async Task ReplaceImageAsync(
        HallImage hallImage,
        UpdateHallImageCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(
            request.ImageStream);

        // ---------------------------------------------------------
        // 1. Get file extension
        // ---------------------------------------------------------

        var extension =
            Path.GetExtension(
                request.FileName!)
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new InvalidOperationException(
                "Unable to determine image file extension.");
        }

        // ---------------------------------------------------------
        // 2. Generate new original object key
        // ---------------------------------------------------------

        var newImageKey =
            $"halls/{hallImage.HallId}/" +
            $"{hallImage.HallImageId}{extension}";

        // ---------------------------------------------------------
        // 3. Reset stream
        // ---------------------------------------------------------

        if (request.ImageStream.CanSeek)
        {
            request.ImageStream.Position = 0;
        }

        // ---------------------------------------------------------
        // 4. Upload new ORIGINAL
        // ---------------------------------------------------------

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
            // The database has not been changed yet.
            // R2 cleanup is attempted below.
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

        // ---------------------------------------------------------
        // 5. Update entity
        //
        // ThumbnailUrl becomes NULL because the new thumbnail
        // will be generated asynchronously by RabbitMQ.
        // ---------------------------------------------------------

        hallImage.Update(
            imageUrl: newImageKey,
            thumbnailUrl: null,
            displayOrder: request.DisplayOrder,
            isCoverImage: request.IsCoverImage);

        hallImage.SetActive(
            request.IsActive);
    }

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
            // Cleanup failure must not fail the successful update.
        }
    }
}