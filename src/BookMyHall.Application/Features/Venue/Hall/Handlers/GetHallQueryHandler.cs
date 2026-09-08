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
    : IRequestHandler<
        GetHallQuery,
        ApiResponse<PaginatedResult<HallListView>>>
{
    public async Task<ApiResponse<PaginatedResult<HallListView>>> Handle(
        GetHallQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;

        // =============================================================
        // Determine current user's roles
        // =============================================================

        var isHallOwner =
            currentUser.Roles.Any(role =>
                string.Equals(
                    role,
                    RoleConstants.HallOwner,
                    StringComparison.OrdinalIgnoreCase));

        var isAdmin =
            currentUser.Roles.Any(role =>
                string.Equals(
                    role,
                    RoleConstants.Admin,
                    StringComparison.OrdinalIgnoreCase));

        // =============================================================
        // Determine Hall Owner filter
        //
        // Admin:
        //     Can see all halls.
        //
        // Hall Owner:
        //     Can see only their own halls.
        //
        // Admin + Hall Owner:
        //     Admin takes priority, so all halls are returned.
        // =============================================================

        Guid? hallOwnerId = null;

        if (isHallOwner && !isAdmin)
        {
            if (currentUser.UserId is null)
            {
                return ApiResponse<
                    PaginatedResult<HallListView>>.FailureResponse(
                    "Authenticated user information could not be determined.",
                    HttpStatusCode.Unauthorized);
            }

            hallOwnerId = currentUser.UserId.Value;
        }

        // =============================================================
        // Build cache key
        // =============================================================

        var baseCacheKey =
            CacheKeyBuilder.BuildPaginatedKey<HallListView>(
                CacheKeys.Hall,
                pagination.PageNumber,
                pagination.PageSize,
                pagination.SearchText,
                pagination.SortBy,
                pagination.SortDescending);

        // =============================================================
        // Scope cache by access level
        //
        // Admin / Admin + Hall Owner:
        //     scope = all
        //
        // Hall Owner only:
        //     scope = their UserId
        //
        // This prevents different Hall Owners from sharing cached data.
        // =============================================================

        var cacheScope =
            hallOwnerId?.ToString() ?? "all";

        var cacheKey =
            $"{baseCacheKey}:scope:{cacheScope}";

        // =============================================================
        // Check cache first
        // =============================================================

        var cachedResponse =
            await cacheService.GetAsync<
                PaginatedResult<HallListView>>(
                cacheKey,
                cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<
                PaginatedResult<HallListView>>.SuccessResponse(
                cachedResponse,
                messageHelper.RetrievedEntity(
                    ResourceNames.Entities,
                    EntityKeys.Hall),
                HttpStatusCode.OK);
        }

        // =============================================================
        // Get halls from database
        // =============================================================

        var result =
            await hallRepository.GetAllAsync(
                pagination,
                hallOwnerId,
                cancellationToken);

        var items = result.Items.ToList();

        // =============================================================
        // Generate pre-signed URLs for cover images
        // =============================================================

        foreach (var hall in items)
        {
            if (!string.IsNullOrWhiteSpace(hall.CoverImageUrl))
            {
                hall.CoverImageUrl =
                    await storageService.GetPreSignedUrlAsync(
                        hall.CoverImageUrl,
                        TimeSpan.FromDays(6)
                            .Add(TimeSpan.FromHours(23)),
                        cancellationToken);
            }
        }

        // =============================================================
        // Build response
        // =============================================================

        var response =
            new PaginatedResult<HallListView>
            {
                Items = items,
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };

        // =============================================================
        // Cache response
        // =============================================================

        await cacheService.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(30),
            cancellationToken);

        // =============================================================
        // Return response
        // =============================================================

        return ApiResponse<
            PaginatedResult<HallListView>>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Hall),
            HttpStatusCode.OK);
    }
}