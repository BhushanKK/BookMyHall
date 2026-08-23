using System.Net;
using MediatR;
using FluentValidation;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateHallImageCommandHandler(
    IHallRepository hallRepository,
    IHallImageRepository hallImageRepository,
    IUnitOfWork unitOfWork,
    IR2StorageService r2StorageService,
    IValidator<CreateHallImageCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreateHallImageCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateHallImageCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate request
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<Guid>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        // 2. Verify Hall exists
        var hall = await hallRepository.GetByIdAsync(request.HallId, cancellationToken);

        if (hall is null)
        {
            return ApiResponse<Guid>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.Hall),
                HttpStatusCode.NotFound
            );
        }

        // 3. Generate HallImageId
        var hallImageId = Guid.NewGuid();

        // 4. Get file extension
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

        // 5. Create R2 object key
        var objectKey = $"halls/{request.HallId}/{hallImageId}{extension}";

        try
        {
            // 6. Upload image to Cloudflare R2
            await r2StorageService.UploadAsync(
                request.ImageStream,
                objectKey,
                request.ContentType,
                cancellationToken);

            // 7. Create HallImage entity
            var hallImage = new HallImage(
                hallImageId,
                request.HallId,
                objectKey,
                null,
                request.DisplayOrder,
                request.IsCoverImage,
                null);

            // 8. Save HallImage
            await hallImageRepository.AddAsync(hallImage, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            await cacheService.RemoveByPrefixAsync($"{CacheKeys.HallImagesPaged}:", cancellationToken);
            // 9. Return success
            return ApiResponse<Guid>.SuccessResponse
            (
                hallImage.HallImageId,
                messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.HallImage),
                HttpStatusCode.Created
            );
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<Guid>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.HallImage),
                HttpStatusCode.Conflict
            );
        }
    }
}