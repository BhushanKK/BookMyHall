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
    // CONFIGURATION
    // =========================================================

    private static readonly TimeSpan PreSignedUrlExpiration =
        TimeSpan.FromMinutes(30);

    private static readonly TimeSpan CacheExpiration =
        TimeSpan.FromMinutes(25);


    // =========================================================
    // HANDLE
    // =========================================================

    public async Task<
        ApiResponse<PaginatedResult<HallImageDto>>> Handle(
            GetHallImagesByHallIdQuery request,
            CancellationToken cancellationToken)
    {
        // =====================================================
        // 1. GET PAGINATION
        // =====================================================

        var pagination = request.Pagination;


        // =====================================================
        // 2. BUILD CACHE KEY
        // =====================================================
        //
        // HallImageCacheKeyBuilder MUST generate a key beginning
        // with:
        //
        // hallimages:page:
        //
        // Example:
        //
        // hallimages:page:{hallId}:1:10:...
        //
        // =====================================================

        var cacheKey =
            HallImageCacheKeyBuilder.BuildPaginatedKey(
                request.HallId,
                pagination.PageNumber,
                pagination.PageSize,
                pagination.SearchText,
                pagination.SortBy,
                pagination.SortDescending);


        // =====================================================
        // 3. TRY CACHE
        // =====================================================

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


        // =====================================================
        // 4. GET FRESH DATA FROM DATABASE
        // =====================================================

        var result =
            await hallImageRepository.GetByHallIdAsync(
                request.HallId,
                pagination,
                cancellationToken);


        // =====================================================
        // 5. CHECK RESULT
        // =====================================================

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


        // =====================================================
        // 6. MAP ENTITY -> DTO
        // =====================================================

        var mappedItems =
            mapper.Map<IReadOnlyList<HallImageDto>>(
                result.Items);


        // =====================================================
        // 7. GENERATE PRE-SIGNED URLS
        // =====================================================

        for (var index = 0;
             index < result.Items.Count;
             index++)
        {
            var hallImage =
                result.Items[index];

            var dto =
                mappedItems[index];


            // =================================================
            // ORIGINAL IMAGE
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                    hallImage.ImageUrl))
            {
                var imageUrl =
                    await r2StorageService.GetPreSignedUrlAsync(
                        hallImage.ImageUrl,
                        PreSignedUrlExpiration,
                        cancellationToken);

                dto.ImageUrl =
                    string.IsNullOrWhiteSpace(imageUrl)
                        ? null
                        : imageUrl;
            }
            else
            {
                dto.ImageUrl = null;
            }


            // =================================================
            // THUMBNAIL
            // =================================================

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
                // Thumbnail is generated asynchronously by
                // RabbitMQ.
                dto.ThumbnailUrl = null;
            }
        }


        // =====================================================
        // 8. CREATE PAGINATED RESULT
        // =====================================================

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


        // =====================================================
        // 9. CACHE FRESH RESULT
        // =====================================================

        await cacheService.SetAsync(
            cacheKey,
            mappedResult,
            CacheExpiration,
            cancellationToken);


        // =====================================================
        // 10. RETURN
        // =====================================================

        return ApiResponse<
            PaginatedResult<HallImageDto>>.SuccessResponse(
            mappedResult,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.HallImage),
            HttpStatusCode.OK);
    }
}