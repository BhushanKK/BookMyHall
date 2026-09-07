using System.Net;

using AutoMapper;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using BookMyHall.Domain.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallCoverImageQueryHandler(
    IHallImageRepository hallImageRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IR2StorageService r2StorageService)
    : IRequestHandler<GetHallCoverImageQuery, ApiResponse<HallImageDto>>
{
    private static readonly TimeSpan CacheExpiration =
        TimeSpan.FromMinutes(25);

    private static readonly TimeSpan PreSignedUrlExpiration =
        TimeSpan.FromMinutes(30);

    public async Task<ApiResponse<HallImageDto>> Handle(
        GetHallCoverImageQuery request,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // 1. BUILD CACHE KEY
        // =========================================================

        var cacheKey =
            $"{CacheKeys.HallCoverImage}:{request.HallId}";

        // =========================================================
        // 2. GET CACHED HALL IMAGE
        // =========================================================
        //
        // IMPORTANT:
        //
        // The cache must NOT contain pre-signed R2 URLs.
        //
        // We cache HallImage entity information/object keys and
        // generate fresh URLs below.
        //
        // =========================================================

        var cachedHallImage =
            await cacheService.GetAsync<HallImage>(
                cacheKey,
                cancellationToken);

        HallImage? coverImage;

        if (cachedHallImage is not null)
        {
            coverImage =
                cachedHallImage;
        }
        else
        {
            // =====================================================
            // 3. GET COVER IMAGE FROM DATABASE
            // =====================================================

            coverImage =
                await hallImageRepository.GetCoverImageAsync(
                    request.HallId,
                    cancellationToken);

            if (coverImage is null)
            {
                return ApiResponse<HallImageDto>.FailureResponse(
                    messageHelper.NotFoundEntity(
                        ResourceNames.Entities,
                        EntityKeys.HallImage),
                    HttpStatusCode.NotFound);
            }

            // =====================================================
            // 4. CACHE ENTITY / OBJECT KEYS
            // =====================================================
            //
            // We deliberately cache the entity instead of the DTO
            // containing signed URLs.
            //
            // =====================================================

            await cacheService.SetAsync(
                cacheKey,
                coverImage,
                CacheExpiration,
                cancellationToken);
        }

        // =========================================================
        // 5. MAP ENTITY → DTO
        // =========================================================

        var response =
            mapper.Map<HallImageDto>(
                coverImage);

        // =========================================================
        // 6. GENERATE FRESH ORIGINAL IMAGE URL
        // =========================================================

        if (!string.IsNullOrWhiteSpace(
                coverImage.ImageUrl))
        {
            var imageUrl =
                await r2StorageService.GetPreSignedUrlAsync(
                    coverImage.ImageUrl,
                    PreSignedUrlExpiration,
                    cancellationToken);

            response.ImageUrl =
                string.IsNullOrWhiteSpace(imageUrl)
                    ? null
                    : imageUrl;
        }
        else
            response.ImageUrl = null;

        // =========================================================
        // 7. GENERATE FRESH THUMBNAIL URL
        // =========================================================

        if (!string.IsNullOrWhiteSpace(
                coverImage.ThumbnailUrl))
        {
            var thumbnailUrl =
                await r2StorageService.GetPreSignedUrlAsync(
                    coverImage.ThumbnailUrl,
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
            // Thumbnail may not have been generated yet.
            response.ThumbnailUrl = null;
        }

        // =========================================================
        // 8. RETURN RESPONSE
        // =========================================================
        //
        // DO NOT CACHE `response` HERE.
        //
        // The response contains temporary R2 signed URLs.
        //
        // =========================================================

        return ApiResponse<HallImageDto>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.HallImage),
            HttpStatusCode.OK);
    }
}