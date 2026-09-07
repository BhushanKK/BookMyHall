using System.Net;

using AutoMapper;
using MediatR;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;

using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;

using BookMyHall.Domain.Venue;

using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallImageByIdQueryHandler(
    IHallImageRepository hallImageRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IR2StorageService r2StorageService)
    : IRequestHandler<
        GetHallImageByIdQuery,
        ApiResponse<HallImageDto>>
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
        // 1. BUILD CACHE KEY
        // =========================================================

        var cacheKey =
            HallImageCacheKeyBuilder.BuildImageKey(
                request.HallImageId);


        // =========================================================
        // 2. GET FROM CACHE
        // =========================================================

        var cachedImage =
            await cacheService.GetAsync<HallImageCacheItem>(
                cacheKey,
                cancellationToken);


        HallImageCacheItem? cacheItem;


        // =========================================================
        // 3. CACHE HIT
        // =========================================================

        if (cachedImage is not null)
        {
            cacheItem = cachedImage;
        }
        else
        {
            // =====================================================
            // 4. CACHE MISS → DATABASE
            // =====================================================

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


            // =====================================================
            // 5. MAP ENTITY → CACHE MODEL
            // =====================================================

            cacheItem =
                MapToCacheItem(hallImage);


            // =====================================================
            // 6. STORE CACHE MODEL
            // =====================================================
            //
            // IMPORTANT:
            //
            // We are NOT caching:
            //
            //     HallImageDto
            //
            // and we are NOT caching:
            //
            //     pre-signed R2 URLs
            //
            // =====================================================

            await cacheService.SetAsync(
                cacheKey,
                cacheItem,
                CacheExpiration,
                cancellationToken);
        }


        // =========================================================
        // 7. MAP CACHE MODEL → DTO
        // =========================================================

        var response =
            new HallImageDto
            {
                HallImageId = cacheItem.HallImageId,
                HallId = cacheItem.HallId,
                ImageUrl = null,
                ThumbnailUrl = null,
                DisplayOrder = cacheItem.DisplayOrder,
                IsCoverImage = cacheItem.IsCoverImage,
                IsActive = cacheItem.IsActive
            };


        // =========================================================
        // 8. VALIDATE ORIGINAL IMAGE OBJECT KEY
        // =========================================================

        if (string.IsNullOrWhiteSpace(cacheItem.ImageUrl))
        {
            return ApiResponse<HallImageDto>.FailureResponse(
                "Hall image object key is missing.",
                HttpStatusCode.InternalServerError);
        }


        // =========================================================
        // 9. GENERATE FRESH ORIGINAL IMAGE URL
        // =========================================================

        var imageUrl =
            await r2StorageService.GetPreSignedUrlAsync(
                cacheItem.ImageUrl,
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
        // 10. GENERATE FRESH THUMBNAIL URL
        // =========================================================

        if (!string.IsNullOrWhiteSpace(
                cacheItem.ThumbnailUrl))
        {
            var thumbnailUrl =
                await r2StorageService.GetPreSignedUrlAsync(
                    cacheItem.ThumbnailUrl,
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
            // Thumbnail may still be generated by RabbitMQ.
            response.ThumbnailUrl = null;
        }


        // =========================================================
        // 11. RETURN RESPONSE
        // =========================================================
        //
        // DO NOT CACHE response.
        //
        // response contains temporary R2 signed URLs.
        //
        // =========================================================

        return ApiResponse<HallImageDto>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.HallImage),
            HttpStatusCode.OK);
    }


    // =============================================================
    // MAP DOMAIN ENTITY → CACHE MODEL
    // =============================================================

    private static HallImageCacheItem MapToCacheItem(
        HallImage hallImage)
    {
        return new HallImageCacheItem
        {
            HallImageId =
                hallImage.HallImageId,

            HallId =
                hallImage.HallId,

            ImageUrl =
                hallImage.ImageUrl,

            ThumbnailUrl =
                hallImage.ThumbnailUrl,

            DisplayOrder =
                hallImage.DisplayOrder,

            IsCoverImage =
                hallImage.IsCoverImage,

            IsActive =
                hallImage.IsActive,

            IsDeleted =
                hallImage.IsDeleted,

            
        };
    }
}