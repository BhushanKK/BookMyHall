using System.Net;

using AutoMapper;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
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
    private static readonly TimeSpan PreSignedUrlExpiration =
        TimeSpan.FromMinutes(30);

    public async Task<ApiResponse<HallImageDto>> Handle(
        GetHallCoverImageQuery request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Cache key
        // ---------------------------------------------------------

        var cacheKey =
            $"{CacheKeys.HallCoverImage}:{request.HallId}";

        // ---------------------------------------------------------
        // 2. Check cache
        // ---------------------------------------------------------

        var cachedHallImage =
            await cacheService.GetAsync<HallImageDto>(
                cacheKey,
                cancellationToken);

        if (cachedHallImage is not null)
        {
            return ApiResponse<HallImageDto>.SuccessResponse(
                cachedHallImage,
                messageHelper.RetrievedEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallImage),
                HttpStatusCode.OK);
        }

        // ---------------------------------------------------------
        // 3. Get cover image from database
        // ---------------------------------------------------------

        var coverImage =
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

        // ---------------------------------------------------------
        // 4. Map entity → DTO
        // ---------------------------------------------------------

        var response =
            mapper.Map<HallImageDto>(coverImage);

        // ---------------------------------------------------------
        // 5. Generate pre-signed URL for ORIGINAL image
        // ---------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(coverImage.ImageUrl))
        {
            var imageUrl =
                await r2StorageService.GetPreSignedUrlAsync(
                    coverImage.ImageUrl,
                    PreSignedUrlExpiration,
                    cancellationToken);

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                response.ImageUrl = imageUrl;
            }
        }

        // ---------------------------------------------------------
        // 6. Generate pre-signed URL for THUMBNAIL
        // ---------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(coverImage.ThumbnailUrl))
        {
            var thumbnailUrl = await r2StorageService.GetPreSignedUrlAsync
            (
                coverImage.ThumbnailUrl,
                PreSignedUrlExpiration,
                cancellationToken
            );

            response.ThumbnailUrl = string.IsNullOrWhiteSpace(thumbnailUrl)
            ? null : thumbnailUrl;
        }
        else
        {
            // Thumbnail is generated asynchronously by RabbitMQ
            response.ThumbnailUrl = null;
        }

        // ---------------------------------------------------------
        // 7. Cache response containing pre-signed URLs
        // ---------------------------------------------------------

        await cacheService.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(25),
            cancellationToken);

        // ---------------------------------------------------------
        // 8. Return response
        // ---------------------------------------------------------

        return ApiResponse<HallImageDto>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.HallImage),
            HttpStatusCode.OK);
    }
}