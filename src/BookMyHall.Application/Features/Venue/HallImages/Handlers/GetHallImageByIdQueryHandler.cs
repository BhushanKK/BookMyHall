using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallImageByIdQueryHandler(
    IHallImageRepository hallImageRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IR2StorageService r2StorageService)
    : IRequestHandler<GetHallImageByIdQuery, ApiResponse<HallImageDto>>
{
    private static readonly TimeSpan PreSignedUrlExpiration = TimeSpan.FromMinutes(30);
    public async Task<ApiResponse<HallImageDto>> Handle(
        GetHallImageByIdQuery request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Cache key
        // ---------------------------------------------------------

        var cacheKey = $"{CacheKeys.HallImage}:{request.HallImageId}";

        // ---------------------------------------------------------
        // 2. Check cache
        // ---------------------------------------------------------

        var cachedHallImage = await cacheService.GetAsync<HallImageDto>(cacheKey, cancellationToken);

        if (cachedHallImage is not null)
        {
            return ApiResponse<HallImageDto>.SuccessResponse
            (
                cachedHallImage,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallImage),
                HttpStatusCode.OK
            );
        }

        // ---------------------------------------------------------
        // 3. Get Hall Image
        // ---------------------------------------------------------

        var hallImage = await hallImageRepository.GetByIdAsync(request.HallImageId, cancellationToken);

        if (hallImage is null)
        {
            return ApiResponse<HallImageDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.HallImage),
                HttpStatusCode.NotFound
            );
        }

        // ---------------------------------------------------------
        // 4. Map entity → DTO
        // ---------------------------------------------------------

        var response = mapper.Map<HallImageDto>(hallImage);

        // ---------------------------------------------------------
        // 5. Generate pre-signed URL for ORIGINAL image
        // ---------------------------------------------------------

        if (string.IsNullOrWhiteSpace(hallImage.ImageUrl))
        {
            return ApiResponse<HallImageDto>.FailureResponse
            (
                "Hall image object key is missing.",
                HttpStatusCode.InternalServerError
            );
        }

        var imageUrl = await r2StorageService.GetPreSignedUrlAsync
        (
            hallImage.ImageUrl,
            PreSignedUrlExpiration,
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return ApiResponse<HallImageDto>.FailureResponse
            (
                "Unable to generate pre-signed URL for the Hall image.",
                HttpStatusCode.InternalServerError
            );
        }

        response.ImageUrl = imageUrl;

        // ---------------------------------------------------------
        // 6. Generate pre-signed URL for THUMBNAIL
        // ---------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(hallImage.ThumbnailUrl))
        {
            var thumbnailUrl = await r2StorageService.GetPreSignedUrlAsync
            (
                hallImage.ThumbnailUrl,
                PreSignedUrlExpiration,
                cancellationToken
            );

            response.ThumbnailUrl = string.IsNullOrWhiteSpace(thumbnailUrl)
                ? null
                : thumbnailUrl;
        }
        else
        {
            // Thumbnail is generated asynchronously by RabbitMQ.
            response.ThumbnailUrl = null;
        }

        // ---------------------------------------------------------
        // 7. Cache DTO with pre-signed URLs
        // ---------------------------------------------------------

        // URL expires in 30 minutes.
        // Cache slightly less than that to avoid serving an expired URL.
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(25), cancellationToken);

        // ---------------------------------------------------------
        // 8. Return response
        // ---------------------------------------------------------

        return ApiResponse<HallImageDto>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallImage), HttpStatusCode.OK
        );
    }
}