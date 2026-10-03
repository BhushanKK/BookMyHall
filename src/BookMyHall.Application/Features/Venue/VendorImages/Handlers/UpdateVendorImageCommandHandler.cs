using System.Net;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Messaging;
using BookMyHall.Contracts.Venue;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateVendorImageCommandHandler(
    IVendorImageRepository repository,
    IVendorServiceRepository vendorServiceRepository,
    IUnitOfWork unitOfWork,
    IR2StorageService storage,
    IMessagePublisher messagePublisher,
    ICacheService cacheService,
    IMessageHelper messageHelper)
    : IRequestHandler<UpdateVendorImageCommand, ApiResponse<VendorImageDto>>
{
    private const long MaxFileSize = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];

    public async Task<ApiResponse<VendorImageDto>> Handle(
        UpdateVendorImageCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DisplayOrder <= 0)
        {
            return ApiResponse<VendorImageDto>.FailureResponse(
                "Display order must be greater than zero.",
                HttpStatusCode.BadRequest);
        }

        var image = await repository.GetByIdAsync(request.VendorImageId, cancellationToken);
        if (image is null)
        {
            return ApiResponse<VendorImageDto>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.VendorImage),
                HttpStatusCode.NotFound);
        }

        var vendorServiceId = request.VendorServiceId ?? image.VendorServiceId;
        if (request.VendorServiceId.HasValue)
        {
            if (vendorServiceId == Guid.Empty)
            {
                return ApiResponse<VendorImageDto>.FailureResponse("Vendor service ID must not be empty.", HttpStatusCode.BadRequest);
            }

            var service = await vendorServiceRepository.GetByIdAsync(request.VendorServiceId.Value, cancellationToken);
            if (service is null || service.IsDeleted || !service.IsActive)
            {
                return ApiResponse<VendorImageDto>.FailureResponse(
                    messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.VendorService),
                    HttpStatusCode.NotFound);
            }

            if (service.VendorId != image.VendorId)
            {
                return ApiResponse<VendorImageDto>.FailureResponse("Vendor service does not belong to this vendor.", HttpStatusCode.BadRequest);
            }
        }

        var replacingImage = request.ImageStream is not null;
        if (replacingImage &&
            (string.IsNullOrWhiteSpace(request.FileName) ||
             string.IsNullOrWhiteSpace(request.ContentType) ||
             request.FileSize is <= 0 or > MaxFileSize ||
             !AllowedExtensions.Contains(Path.GetExtension(request.FileName), StringComparer.OrdinalIgnoreCase) ||
             !AllowedContentTypes.Contains(request.ContentType, StringComparer.OrdinalIgnoreCase)))
        {
            return ApiResponse<VendorImageDto>.FailureResponse(
                "Provide a valid JPG, PNG, or WEBP image up to 5 MB.",
                HttpStatusCode.BadRequest);
        }

        var oldImageKey = image.ImageUrl;
        var oldThumbnailKey = image.ThumbnailUrl;
        string? newImageKey = null;

        if (replacingImage)
        {
            var extension = Path.GetExtension(request.FileName!).ToLowerInvariant();
            newImageKey = $"vendors/{image.VendorId}/{image.VendorImageId}{extension}";
            try
            {
                if (request.ImageStream!.CanSeek)
                {
                    request.ImageStream.Position = 0;
                }

                await storage.UploadAsync(
                    request.ImageStream!,
                    newImageKey,
                    request.ContentType!,
                    cancellationToken);
            }
            catch
            {
                await DeleteStorageObjectSafelyAsync(newImageKey);
                throw;
            }

            image.ReplaceImage(newImageKey);
        }

        var isCoverImage = request.IsActive && request.IsCoverImage;
        if (isCoverImage)
        {
            await repository.ClearOtherCoverImagesAsync(
                image.VendorId,
                image.VendorImageId,
                cancellationToken,
                vendorServiceId);
        }

        image.VendorServiceId = vendorServiceId;
        image.UpdateMetadata(request.DisplayOrder, isCoverImage, request.IsActive);

        try
        {
            await repository.UpdateAsync(image, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            if (newImageKey is not null)
            {
                await DeleteStorageObjectSafelyAsync(newImageKey);
            }

            return ApiResponse<VendorImageDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.VendorImage),
                HttpStatusCode.Conflict);
        }
        catch
        {
            if (newImageKey is not null)
            {
                await DeleteStorageObjectSafelyAsync(newImageKey);
            }

            throw;
        }

        await VendorImageCacheInvalidator.InvalidateAsync(
            cacheService,
            image.VendorId,
            image.VendorImageId,
            cancellationToken);

        if (replacingImage)
        {
            await messagePublisher.PublishAsync(
                new VendorImageUploadedMessage(image.VendorImageId, image.VendorId, image.ImageUrl),
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(oldImageKey) && oldImageKey != image.ImageUrl)
            {
                await DeleteStorageObjectSafelyAsync(oldImageKey);
            }

            if (!string.IsNullOrWhiteSpace(oldThumbnailKey))
            {
                await DeleteStorageObjectSafelyAsync(oldThumbnailKey);
            }
        }

        var response = await VendorImageDtoFactory.CreateAsync(image, storage, cancellationToken);
        return ApiResponse<VendorImageDto>.SuccessResponse(
            response,
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.VendorImage),
            HttpStatusCode.OK);
    }

    private async Task DeleteStorageObjectSafelyAsync(string objectKey)
    {
        try
        {
            await storage.DeleteAsync(objectKey, CancellationToken.None);
        }
        catch
        {
        }
    }
}
