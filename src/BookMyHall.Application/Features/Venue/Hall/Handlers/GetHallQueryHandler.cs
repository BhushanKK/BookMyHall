using MediatR;
using System.Net;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Domain.Venue;
using BookMyHall.Domain.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallQueryHandler(
    IHallRepository hallRepository,
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IR2StorageService storageService,
    ICurrentUser currentUser)
    : IRequestHandler<GetHallQuery, ApiResponse<PaginatedResult<HallListView>>>
{
    public async Task<ApiResponse<PaginatedResult<HallListView>>> Handle(
        GetHallQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;

        // =============================================================
        // Determine current user's role
        // =============================================================

        var isHallOwner = currentUser.Roles.Any(role => string.Equals(role, RoleConstants.HallOwner, StringComparison.OrdinalIgnoreCase));

        var isAdmin = currentUser.Roles.Any(role => string.Equals(role, RoleConstants.Admin, StringComparison.OrdinalIgnoreCase));

        // =============================================================
        // Determine Hall Owner filter
        // =============================================================

        Guid? hallOwnerId = null;

        // Admin can see all halls.
        // Hall Owner can see only their own halls.
        // If user has both roles, Admin takes priority.
        if (isHallOwner && !isAdmin)
        {
            if (currentUser.UserId is null)
            {
                return ApiResponse<PaginatedResult<HallListView>>.FailureResponse
                (
                    "Authenticated user information could not be determined.",
                    HttpStatusCode.Unauthorized
                );
            }

            hallOwnerId = currentUser.UserId.Value;
        }

        // =============================================================
        // Cache Key
        // =============================================================

        var baseCacheKey =
            CacheKeyBuilder.BuildPaginatedKey<HallListView>(
                CacheKeys.Hall,
                pagination.PageNumber,
                pagination.PageSize,
                pagination.SearchText,
                pagination.SortBy,
                pagination.SortDescending);

        // IMPORTANT:
        // Admin and different Hall Owners must not share the same cache.
        var cacheScope = hallOwnerId?.ToString() ?? "all";
        var cacheKey = $"{baseCacheKey}:scope:{cacheScope}";

        // =============================================================
        // Check cache first
        // =============================================================

        var cachedResponse = await cacheService.GetAsync<PaginatedResult<HallListView>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResult<HallListView>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Hall),
                HttpStatusCode.OK
            );
        }

        // =============================================================
        // Get halls from database
        // =============================================================

        var result = await hallRepository.GetAllAsync(pagination, hallOwnerId, cancellationToken);
        var items = result.Items.ToList();

        // =============================================================
        // Generate presigned URLs for cover images
        // =============================================================

        foreach (var hall in items)
        {
            if (!string.IsNullOrWhiteSpace(hall.CoverImageUrl))
            {
                hall.CoverImageUrl = await storageService.GetPreSignedUrlAsync
                (
                    hall.CoverImageUrl,
                    TimeSpan.FromDays(6).Add(TimeSpan.FromHours(23)),
                    cancellationToken
                );
            }
        }

        // =============================================================
        // Build response
        // =============================================================

        var response = new PaginatedResult<HallListView>
        {
            Items = items,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        // =============================================================
        // Cache response
        // =============================================================

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        // =============================================================
        // Return response
        // =============================================================

        return ApiResponse<PaginatedResult<HallListView>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Hall),
            HttpStatusCode.OK
        );
    }
}