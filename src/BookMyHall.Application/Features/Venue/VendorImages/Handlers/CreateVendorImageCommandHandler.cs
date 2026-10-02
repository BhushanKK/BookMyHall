using System.Net;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Messaging;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateVendorImageCommandHandler(
    IVendorRepository vendorRepository,
    IVendorImageRepository vendorImageRepository,
    IUnitOfWork unitOfWork,
    IR2StorageService storage,
    IMessagePublisher messagePublisher,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<CreateVendorImageCommand, ApiResponse<Guid>>
{
    private const long MaxFileSize = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];

    public async Task<ApiResponse<Guid>> Handle(CreateVendorImageCommand request, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(request.FileName);
        if (request.VendorId == Guid.Empty || request.ImageStream is null ||
            request.FileSize is <= 0 or > MaxFileSize || request.DisplayOrder <= 0 ||
            !AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase) ||
            !AllowedContentTypes.Contains(request.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            return ApiResponse<Guid>.FailureResponse(
                "Provide a valid JPG, PNG, or WEBP image up to 5 MB and a positive display order.",
                HttpStatusCode.BadRequest);
        }

        var vendor = await vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor is null)
        {
            return ApiResponse<Guid>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.Vendor),
                HttpStatusCode.NotFound);
        }

        var imageId = Guid.NewGuid();
        var objectKey = $"vendors/{request.VendorId}/{imageId}{extension.ToLowerInvariant()}";
        var uploaded = false;

        try
        {
            using var buffer = new MemoryStream();
            await request.ImageStream.CopyToAsync(buffer, cancellationToken);
            var bytes = buffer.ToArray();

            await using (var uploadStream = new MemoryStream(bytes, writable: false))
            {
                await storage.UploadAsync(uploadStream, objectKey, request.ContentType, cancellationToken);
            }
            uploaded = true;

            if (request.IsCoverImage)
            {
                await vendorImageRepository.ClearOtherCoverImagesAsync(
                    request.VendorId,
                    null,
                    cancellationToken);
            }

            await vendorImageRepository.AddAsync(new VendorImage
            {
                VendorImageId = imageId,
                VendorId = request.VendorId,
                ImageUrl = objectKey,
                DisplayOrder = request.DisplayOrder,
                IsCoverImage = request.IsCoverImage,
                IsActive = true,
                CreatedDate = DateTimeOffset.UtcNow
            }, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            await CleanupUploadAsync(objectKey, uploaded);
            return ApiResponse<Guid>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.VendorImage),
                HttpStatusCode.Conflict);
        }
        catch
        {
            await CleanupUploadAsync(objectKey, uploaded);
            throw;
        }

        await VendorImageCacheInvalidator.InvalidateAsync(
            cacheService,
            request.VendorId,
            imageId,
            cancellationToken);

        await messagePublisher.PublishAsync(
            new VendorImageUploadedMessage(imageId, request.VendorId, objectKey),
            cancellationToken);

        return ApiResponse<Guid>.SuccessResponse(
            imageId,
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.VendorImage),
            HttpStatusCode.Created);
    }

    private async Task CleanupUploadAsync(string objectKey, bool uploaded)
    {
        if (!uploaded)
        {
            return;
        }

        try
        {
            await storage.DeleteAsync(objectKey, CancellationToken.None);
        }
        catch
        {
        }
    }
}