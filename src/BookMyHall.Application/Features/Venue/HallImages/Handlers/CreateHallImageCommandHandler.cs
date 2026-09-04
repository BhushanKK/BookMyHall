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
using FluentValidation;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateHallImageCommandHandler(
    IHallRepository hallRepository,
    IHallImageRepository hallImageRepository,
    IUnitOfWork unitOfWork,
    IR2StorageService r2StorageService,
    IValidator<CreateHallImageCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IMessagePublisher messagePublisher)
    : IRequestHandler<CreateHallImageCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(
        CreateHallImageCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate request
        // ---------------------------------------------------------
        var validationResult =
            await validator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(
                " | ",
                validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<Guid>.FailureResponse(
                message,
                HttpStatusCode.BadRequest);
        }

        // ---------------------------------------------------------
        // 2. Verify Hall exists
        // ---------------------------------------------------------
        var hall = await hallRepository.GetByIdAsync(
            request.HallId,
            cancellationToken);

        if (hall is null)
        {
            return ApiResponse<Guid>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.Hall),
                HttpStatusCode.NotFound);
        }

        // ---------------------------------------------------------
        // 3. Generate HallImageId
        // ---------------------------------------------------------
        var hallImageId = Guid.NewGuid();

        // ---------------------------------------------------------
        // 4. Get file extension
        // ---------------------------------------------------------
        var extension =
            Path.GetExtension(request.FileName)
                .ToLowerInvariant();

        // ---------------------------------------------------------
        // 5. R2 object key
        // ---------------------------------------------------------
        var objectKey =
            $"halls/{request.HallId}/{hallImageId}{extension}";

        var originalUploaded = false;

        try
        {
            // -----------------------------------------------------
            // 6. Copy uploaded file into memory
            //
            // We create a byte[] so the request stream can be
            // safely consumed by the R2 SDK without affecting
            // the original request stream.
            // -----------------------------------------------------
            await using var requestStream =
                request.ImageStream;

            using var inputMemoryStream =
                new MemoryStream();

            await requestStream.CopyToAsync(
                inputMemoryStream,
                cancellationToken);

            var imageBytes =
                inputMemoryStream.ToArray();

            // -----------------------------------------------------
            // 7. Upload ORIGINAL image to R2
            // -----------------------------------------------------
            await using (
                var originalStream =
                    new MemoryStream(
                        imageBytes,
                        writable: false))
            {
                await r2StorageService.UploadAsync(
                    originalStream,
                    objectKey,
                    request.ContentType,
                    cancellationToken);
            }

            originalUploaded = true;

            // -----------------------------------------------------
            // 8. Create HallImage entity
            //
            // ThumbnailUrl is intentionally NULL.
            //
            // HallImageThumbnailConsumer will generate the
            // thumbnail asynchronously.
            // -----------------------------------------------------
            var hallImage = new HallImage(
                hallImageId,
                request.HallId,
                objectKey,
                null,
                request.DisplayOrder,
                request.IsCoverImage,
                null);

            // -----------------------------------------------------
            // 9. Save database record
            // -----------------------------------------------------
            await hallImageRepository.AddAsync(
                hallImage,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(
                cancellationToken);

            // -----------------------------------------------------
            // 10. Publish RabbitMQ event
            // -----------------------------------------------------
            var message = new HallImageUploadedMessage(
                HallImageId: hallImage.HallImageId,
                HallId: hallImage.HallId,
                ObjectKey: objectKey);

            await messagePublisher.PublishAsync(
                message,
                cancellationToken);

            // -----------------------------------------------------
            // 11. Clear Hall image cache
            // -----------------------------------------------------
            await cacheService.RemoveByPrefixAsync(
                $"{CacheKeys.HallImagesPaged}:",
                cancellationToken);

            // -----------------------------------------------------
            // 12. Return success
            // -----------------------------------------------------
            return ApiResponse<Guid>.SuccessResponse(
                hallImage.HallImageId,
                messageHelper.AddedEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallImage),
                HttpStatusCode.Created);
        }
        catch (DuplicateRecordException)
        {
            await CleanupR2ObjectAsync(
                objectKey,
                originalUploaded);

            return ApiResponse<Guid>.FailureResponse(
                messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallImage),
                HttpStatusCode.Conflict);
        }
        catch
        {
            await CleanupR2ObjectAsync(
                objectKey,
                originalUploaded);

            throw;
        }
    }

    private async Task CleanupR2ObjectAsync(
        string objectKey,
        bool originalUploaded)
    {
        try
        {
            if (originalUploaded)
            {
                await r2StorageService.DeleteAsync(
                    objectKey,
                    CancellationToken.None);
            }
        }
        catch
        {
            // Do not hide the original exception.
        }
    }
}