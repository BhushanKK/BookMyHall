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
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallImageByIdQueryHandler(
    IHallImageRepository hallImageRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IR2StorageService r2StorageService)
    : IRequestHandler<GetHallImageByIdQuery, ApiResponse<HallImageDto>>
{
    private static readonly TimeSpan CacheExpiration =
        TimeSpan.FromMinutes(25);

    private static readonly TimeSpan PreSignedUrlExpiration =
        TimeSpan.FromMinutes(30);

    public async Task<ApiResponse<HallImageDto>> Handle(
        GetHallImageByIdQuery request,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // 1. CACHE KEY
        // =========================================================

        var cacheKey =
            $"{CacheKeys.HallImage}:{request.HallImageId}";

        // =========================================================
        // 2. CHECK CACHE
        // =========================================================
        //
        // IMPORTANT:
        //
        // We cache the HallImage entity/object-key information.
        //
        // We DO NOT cache HallImageDto because the DTO contains
        // temporary R2 pre-signed URLs.
        //
        // =========================================================

        var cachedHallImage =
            await cacheService.GetAsync<HallImage>(
                cacheKey,
                cancellationToken);

        HallImage? hallImage;

        if (cachedHallImage is not null)
        {
            hallImage =
                cachedHallImage;
        }
        else
        {
            // =====================================================
            // 3. GET HALL IMAGE FROM DATABASE
            // =====================================================

            hallImage =
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

            // =====================================================
            // 4. CACHE HALL IMAGE DATA
            // =====================================================
            //
            // Only persistent information/object keys are cached.
            //
            // No pre-signed URLs are stored in cache.
            //
            // =====================================================

            await cacheService.SetAsync(
                cacheKey,
                hallImage,
                CacheExpiration,
                cancellationToken);
        }

        // =========================================================
        // 5. MAP ENTITY → DTO
        // =========================================================

        var response =
            mapper.Map<HallImageDto>(
                hallImage);

        // =========================================================
        // 6. VALIDATE ORIGINAL OBJECT KEY
        // =========================================================

        if (string.IsNullOrWhiteSpace(
                hallImage.ImageUrl))
        {
            return ApiResponse<HallImageDto>.FailureResponse(
                "Hall image object key is missing.",
                HttpStatusCode.InternalServerError);
        }

        // =========================================================
        // 7. GENERATE FRESH ORIGINAL IMAGE URL
        // =========================================================

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
        // 8. GENERATE FRESH THUMBNAIL URL
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
            // Thumbnail may still be generating through RabbitMQ.
            response.ThumbnailUrl = null;
        }

        // =========================================================
        // 9. DO NOT CACHE RESPONSE
        // =========================================================
        //
        // response contains:
        //
        //     response.ImageUrl
        //     response.ThumbnailUrl
        //
        // Both are temporary pre-signed URLs.
        //
        // Therefore:
        //
        //     DO NOT:
        //
        //     cacheService.SetAsync(cacheKey, response, ...)
        //
        // =========================================================

        // =========================================================
        // 10. RETURN RESPONSE
        // =========================================================

        return ApiResponse<HallImageDto>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.HallImage),
            HttpStatusCode.OK);
    }
}