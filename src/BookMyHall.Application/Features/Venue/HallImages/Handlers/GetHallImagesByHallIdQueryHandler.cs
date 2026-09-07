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

public sealed class GetHallImagesByHallIdQueryHandler(
    IHallImageRepository hallImageRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IR2StorageService r2StorageService)
    : IRequestHandler<
        GetHallImagesByHallIdQuery,
        ApiResponse<PaginatedResult<HallImageDto>>>
{
    // =========================================================
    // Configuration
    // =========================================================

    private static readonly TimeSpan PreSignedUrlExpiration =
        TimeSpan.FromMinutes(30);

    private static readonly TimeSpan CacheExpiration =
        TimeSpan.FromMinutes(25);


    // =========================================================
    // Handle
    // =========================================================

    public async Task<
        ApiResponse<PaginatedResult<HallImageDto>>> Handle(
            GetHallImagesByHallIdQuery request,
            CancellationToken cancellationToken)
    {
        // -----------------------------------------------------
        // 1. Pagination
        // -----------------------------------------------------

        var pagination = request.Pagination;


        // -----------------------------------------------------
        // 2. Build Hall-specific cache key
        // -----------------------------------------------------
        //
        // IMPORTANT:
        // HallId MUST be part of the cache key.
        //
        // Otherwise:
        //
        // Hall A -> cache key X
        // Hall B -> cache key X
        //
        // Hall B can receive Hall A's cached images.
        //
        // -----------------------------------------------------

        var cacheKey =
            HallImageCacheKeyBuilder.BuildPaginatedKey(
                request.HallId,
                pagination.PageNumber,
                pagination.PageSize,
                pagination.SearchText,
                pagination.SortBy,
                pagination.SortDescending);


        // -----------------------------------------------------
        // 3. Check cache
        // -----------------------------------------------------

        var cachedResult =
            await cacheService.GetAsync<
                PaginatedResult<HallImageDto>>(
                cacheKey,
                cancellationToken);

        if (cachedResult is not null)
        {
            return ApiResponse<
                PaginatedResult<HallImageDto>>.SuccessResponse(
                cachedResult,
                messageHelper.RetrievedEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallImage),
                HttpStatusCode.OK);
        }


        // -----------------------------------------------------
        // 4. Get images from database
        // -----------------------------------------------------

        var result =
            await hallImageRepository.GetByHallIdAsync(
                request.HallId,
                pagination,
                cancellationToken);


        // -----------------------------------------------------
        // 5. Handle no images
        // -----------------------------------------------------

        if (result.Items is null ||
            result.Items.Count == 0)
        {
            return ApiResponse<
                PaginatedResult<HallImageDto>>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallImage),
                HttpStatusCode.NotFound);
        }


        // -----------------------------------------------------
        // 6. Map entities -> DTOs
        // -----------------------------------------------------

        var mappedItems =
            mapper.Map<IReadOnlyList<HallImageDto>>(
                result.Items);


        // -----------------------------------------------------
        // 7. Generate pre-signed URLs
        // -----------------------------------------------------

        for (var index = 0;
             index < result.Items.Count;
             index++)
        {
            var hallImage =
                result.Items[index];

            var dto =
                mappedItems[index];


            // -------------------------------------------------
            // Original image
            // -------------------------------------------------

            if (!string.IsNullOrWhiteSpace(
                    hallImage.ImageUrl))
            {
                var imageUrl =
                    await r2StorageService.GetPreSignedUrlAsync(
                        hallImage.ImageUrl,
                        PreSignedUrlExpiration,
                        cancellationToken);

                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    dto.ImageUrl = imageUrl;
                }
            }


            // -------------------------------------------------
            // Thumbnail image
            // -------------------------------------------------

            if (!string.IsNullOrWhiteSpace(
                    hallImage.ThumbnailUrl))
            {
                var thumbnailUrl =
                    await r2StorageService.GetPreSignedUrlAsync(
                        hallImage.ThumbnailUrl,
                        PreSignedUrlExpiration,
                        cancellationToken);

                dto.ThumbnailUrl =
                    string.IsNullOrWhiteSpace(thumbnailUrl)
                        ? null
                        : thumbnailUrl;
            }
            else
            {
                // Thumbnail may still be processing
                // asynchronously through RabbitMQ.

                dto.ThumbnailUrl = null;
            }
        }


        // -----------------------------------------------------
        // 8. Create paginated response
        // -----------------------------------------------------

        var mappedResult =
            new PaginatedResult<HallImageDto>
            {
                Items = mappedItems,

                TotalCount =
                    result.TotalCount,

                PageNumber =
                    result.PageNumber,

                PageSize =
                    result.PageSize
            };


        // -----------------------------------------------------
        // 9. Store result in cache
        // -----------------------------------------------------

        await cacheService.SetAsync(
            cacheKey,
            mappedResult,
            CacheExpiration,
            cancellationToken);


        // -----------------------------------------------------
        // 10. Return response
        // -----------------------------------------------------

        return ApiResponse<
            PaginatedResult<HallImageDto>>.SuccessResponse(
            mappedResult,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.HallImage),
            HttpStatusCode.OK);
    }
}