using System.Net;
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
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IR2StorageService r2StorageService)
    : IRequestHandler<
        GetHallImagesByHallIdQuery,
        ApiResponse<PaginatedResult<HallImageDto>>>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(25);

    private static readonly TimeSpan PreSignedUrlExpiration = TimeSpan.FromMinutes(30);

    public async Task<ApiResponse<PaginatedResult<HallImageDto>>> Handle(
        GetHallImagesByHallIdQuery request, CancellationToken cancellationToken)
    {
        var pagination = request.Pagination;


        // =========================================================
        // 1. BUILD PAGINATION CACHE KEY
        // =========================================================

        var cacheKey =
            HallImageCacheKeyBuilder.BuildPaginatedKey(
                request.HallId,
                pagination.PageNumber,
                pagination.PageSize,
                pagination.SearchText,
                pagination.SortBy,
                pagination.SortDescending);


        // =========================================================
        // 2. CHECK CACHE
        // =========================================================

        var cachedResult =
            await cacheService.GetAsync<
                PaginatedResult<HallImageCacheItem>>(
                cacheKey,
                cancellationToken);


        PaginatedResult<HallImageCacheItem>? result;


        // =========================================================
        // 3. CACHE HIT
        // =========================================================

        if (cachedResult is not null)
        {
            result = cachedResult;
        }
        else
        {
            // =====================================================
            // 4. CACHE MISS → DATABASE
            // =====================================================

            var databaseResult =
                await hallImageRepository.GetByHallIdAsync(
                    request.HallId,
                    pagination,
                    cancellationToken);


            if (databaseResult.Items is null ||
                databaseResult.Items.Count == 0)
            {
                return ApiResponse<
                    PaginatedResult<HallImageDto>>.FailureResponse(
                    messageHelper.NotFoundEntity(
                        ResourceNames.Entities,
                        EntityKeys.HallImage),
                    HttpStatusCode.NotFound);
            }


            // =====================================================
            // 5. MAP DATABASE ENTITIES → CACHE ITEMS
            // =====================================================

            var cacheItems =
                databaseResult.Items
                    .Select(MapToCacheItem)
                    .ToList();


            result =
                new PaginatedResult<HallImageCacheItem>
                {
                    Items =
                        cacheItems,

                    TotalCount =
                        databaseResult.TotalCount,

                    PageNumber =
                        databaseResult.PageNumber,

                    PageSize =
                        databaseResult.PageSize
                };


            // =====================================================
            // 6. CACHE PERSISTENT DATA ONLY
            // =====================================================

            await cacheService.SetAsync(
                cacheKey,
                result,
                CacheExpiration,
                cancellationToken);
        }


        // =========================================================
        // 7. GENERATE DTOs + FRESH R2 URLs
        // =========================================================

        var responseItems =
            new List<HallImageDto>(
                result.Items.Count);


        foreach (var item in result.Items)
        {
            var dto =
                new HallImageDto
                {
                    HallImageId =
                        item.HallImageId,

                    HallId =
                        item.HallId,

                    ImageUrl =
                        null,

                    ThumbnailUrl =
                        null,

                    DisplayOrder =
                        item.DisplayOrder,

                    IsCoverImage =
                        item.IsCoverImage,

                    IsActive =
                        item.IsActive                    
                };


            // =====================================================
            // ORIGINAL IMAGE
            // =====================================================

            if (!string.IsNullOrWhiteSpace(
                    item.ImageUrl))
            {
                var imageUrl =
                    await r2StorageService.GetPreSignedUrlAsync(
                        item.ImageUrl,
                        PreSignedUrlExpiration,
                        cancellationToken);


                dto.ImageUrl =
                    string.IsNullOrWhiteSpace(imageUrl)
                        ? null
                        : imageUrl;
            }


            // =====================================================
            // THUMBNAIL
            // =====================================================

            if (!string.IsNullOrWhiteSpace(
                    item.ThumbnailUrl))
            {
                var thumbnailUrl =
                    await r2StorageService.GetPreSignedUrlAsync(
                        item.ThumbnailUrl,
                        PreSignedUrlExpiration,
                        cancellationToken);


                dto.ThumbnailUrl =
                    string.IsNullOrWhiteSpace(
                        thumbnailUrl)
                        ? null
                        : thumbnailUrl;
            }


            responseItems.Add(dto);
        }


        // =========================================================
        // 8. BUILD API RESULT
        // =========================================================

        var response =
            new PaginatedResult<HallImageDto>
            {
                Items =
                    responseItems,

                TotalCount =
                    result.TotalCount,

                PageNumber =
                    result.PageNumber,

                PageSize =
                    result.PageSize
            };


        // =========================================================
        // 9. RETURN
        // =========================================================
        //
        // IMPORTANT:
        //
        // response contains fresh R2 pre-signed URLs.
        //
        // NEVER put this response into cache.
        //
        // =========================================================

        return ApiResponse<
            PaginatedResult<HallImageDto>>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.HallImage),
            HttpStatusCode.OK);
    }


    // =============================================================
    // MAP DOMAIN ENTITY → CACHE ITEM
    // =============================================================

    private static HallImageCacheItem MapToCacheItem(
        BookMyHall.Domain.Venue.HallImage hallImage)
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
                hallImage.IsDeleted            
        };
    }
}