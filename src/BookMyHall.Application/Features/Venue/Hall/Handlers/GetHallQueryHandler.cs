using MediatR;
using System.Net;

using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallQueryHandler(
    IHallRepository hallRepository,
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IR2StorageService storageService)
    : IRequestHandler<
        GetHallQuery,
        ApiResponse<PaginatedResult<HallListDto>>>
{
    public async Task<ApiResponse<PaginatedResult<HallListDto>>> Handle(
        GetHallQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;

        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<HallListDto>(
            CacheKeys.Hall,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        // Check cache first
        var cachedResponse =
            await cacheService.GetAsync<
                PaginatedResult<HallListDto>>(
                    cacheKey,
                    cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<
                PaginatedResult<HallListDto>>.SuccessResponse(
                    cachedResponse,
                    messageHelper.RetrievedEntity(
                        ResourceNames.Entities,
                        EntityKeys.Hall),
                    HttpStatusCode.OK);
        }

        // Get halls from database
        var result = await hallRepository.GetAllAsync(
            pagination,
            cancellationToken);

        var items = result.Items.ToList();

        // Generate presigned URLs for cover images
        foreach (var hall in items)
        {
            if (!string.IsNullOrWhiteSpace(hall.CoverImageUrl))
            {
                hall.CoverImageUrl =
                    await storageService.GetPreSignedUrlAsync(
                        hall.CoverImageUrl,
                        TimeSpan.FromDays(6).Add(
                            TimeSpan.FromHours(23)),
                        cancellationToken);
            }
        }

        var response = new PaginatedResult<HallListDto>
        {
            Items = items,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        // Cache the response containing presigned URLs
        await cacheService.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(30),
            cancellationToken);

        return ApiResponse<
            PaginatedResult<HallListDto>>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Hall),
            HttpStatusCode.OK);
    }
}