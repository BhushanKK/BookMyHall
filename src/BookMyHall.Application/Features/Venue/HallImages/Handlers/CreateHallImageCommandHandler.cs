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
    : IRequestHandler<
        CreateHallImageCommand,
        ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(
        CreateHallImageCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // 1. VALIDATE REQUEST
        // =========================================================

        var validationResult =
            await validator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            var message =
                string.Join(
                    " | ",
                    validationResult.Errors
                        .Select(x => x.ErrorMessage));

            return ApiResponse<Guid>.FailureResponse(
                message,
                HttpStatusCode.BadRequest);
        }


        // =========================================================
        // 2. VERIFY HALL EXISTS
        // =========================================================

        var hall =
            await hallRepository.GetByIdAsync(
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


        // =========================================================
        // 3. GENERATE HALL IMAGE ID
        // =========================================================

        var hallImageId =
            Guid.NewGuid();


        // =========================================================
        // 4. GET FILE EXTENSION
        // =========================================================

        var extension =
            Path.GetExtension(
                request.FileName)
                .ToLowerInvariant();


        // =========================================================
        // 5. BUILD R2 OBJECT KEY
        // =========================================================

        var objectKey =
            $"halls/{request.HallId}/{hallImageId}{extension}";


        var originalUploaded =
            false;


        try
        {
            // =====================================================
            // 6. COPY REQUEST STREAM INTO MEMORY
            // =====================================================
            //
            // The incoming HTTP request stream belongs to the
            // ASP.NET request pipeline.
            //
            // Copying it to memory gives the R2 upload its own
            // independent stream.
            //
            // =====================================================

            await using var requestStream =
                request.ImageStream;

            using var inputMemoryStream =
                new MemoryStream();


            await requestStream.CopyToAsync(
                inputMemoryStream,
                cancellationToken);


            var imageBytes =
                inputMemoryStream.ToArray();


            // =====================================================
            // 7. UPLOAD ORIGINAL IMAGE TO R2
            // =====================================================

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


            originalUploaded =
                true;


            // =====================================================
            // 8. CREATE HALL IMAGE ENTITY
            // =====================================================
            //
            // ThumbnailUrl is intentionally NULL.
            //
            // RabbitMQ will generate the thumbnail
            // asynchronously.
            //
            // =====================================================

            var hallImage =
                new HallImage(
                    hallImageId,
                    request.HallId,
                    objectKey,
                    null,
                    request.DisplayOrder,
                    request.IsCoverImage,
                    null);


            // =====================================================
            // 9. SAVE HALL IMAGE TO DATABASE
            // =====================================================

            await hallImageRepository.AddAsync(
                hallImage,
                cancellationToken);


            await unitOfWork.SaveChangesAsync(
                cancellationToken);


            // =====================================================
            // 10. INVALIDATE CACHE
            // =====================================================
            //
            // IMPORTANT:
            //
            // The database has now been successfully updated.
            //
            // We must remove stale cached image metadata before
            // returning the newly-created image.
            //
            // =====================================================

            await InvalidateHallImageCachesAsync(
                request.HallId,
                cancellationToken);


            // =====================================================
            // 11. PUBLISH RABBITMQ EVENT
            // =====================================================
            //
            // Thumbnail generation happens asynchronously.
            //
            // =====================================================

            var message =
                new HallImageUploadedMessage(
                    HallImageId:
                        hallImage.HallImageId,

                    HallId:
                        hallImage.HallId,

                    ObjectKey:
                        objectKey);


            await messagePublisher.PublishAsync(
                message,
                cancellationToken);


            // =====================================================
            // 12. RETURN SUCCESS
            // =====================================================

            return ApiResponse<Guid>.SuccessResponse(
                hallImage.HallImageId,

                messageHelper.AddedEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallImage),

                HttpStatusCode.Created);
        }
        catch (DuplicateRecordException)
        {
            // =====================================================
            // DUPLICATE RECORD
            // =====================================================

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
            // =====================================================
            // GENERAL FAILURE
            // =====================================================

            await CleanupR2ObjectAsync(
                objectKey,
                originalUploaded);

            throw;
        }
    }


    // =============================================================
    // INVALIDATE HALL IMAGE CACHES
    // =============================================================

    private async Task InvalidateHallImageCachesAsync(
        Guid hallId,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // 1. INVALIDATE PAGINATED HALL IMAGE CACHE
        // =========================================================
        //
        // We invalidate ONLY this hall's paginated cache.
        //
        // Example cache keys:
        //
        // hallimages:page:{hallId}:page:1:size:10:search:none:sort:none:desc:true
        //
        // hallimages:page:{hallId}:page:2:size:10:search:none:sort:none:desc:true
        //
        // etc.
        //
        // =========================================================

        await cacheService.RemoveByPrefixAsync(
            HallImageCacheKeyBuilder.BuildHallPaginatedPrefix(
                hallId),
            cancellationToken);


        // =========================================================
        // 2. INVALIDATE HALL COVER IMAGE CACHE
        // =========================================================
        //
        // A newly-created image may be the new cover image.
        //
        // Therefore the existing cover cache must be removed.
        //
        // =========================================================

        await cacheService.RemoveAsync(
            HallImageCacheKeyBuilder.BuildCoverImageKey(
                hallId),
            cancellationToken);
    }


    // =============================================================
    // R2 CLEANUP
    // =============================================================

    private async Task CleanupR2ObjectAsync(
        string objectKey,
        bool originalUploaded)
    {
        try
        {
            // =====================================================
            // Nothing was uploaded to R2.
            // =====================================================

            if (!originalUploaded)
            {
                return;
            }


            // =====================================================
            // DELETE UPLOADED OBJECT
            // =====================================================

            await r2StorageService.DeleteAsync(
                objectKey,
                CancellationToken.None);
        }
        catch
        {
            // =====================================================
            // IMPORTANT:
            //
            // Never hide the original exception because R2 cleanup
            // itself failed.
            //
            // =====================================================
        }
    }
}