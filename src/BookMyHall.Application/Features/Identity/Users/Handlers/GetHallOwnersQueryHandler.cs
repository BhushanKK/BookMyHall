using MediatR;

using BookMyHall.Domain.Constants;
using BookMyHall.Domain.Dtos;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;

using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.HallOwner.Queries;

public sealed class GetHallOwnersQueryHandler(
    IUserRepository userRepository,
    ICacheService cacheService,
    ICurrentUser currentUser)
    : IRequestHandler<
        GetHallOwnersQuery,
        IReadOnlyList<HallOwnerDto>>
{
    public async Task<IReadOnlyList<HallOwnerDto>> Handle(
        GetHallOwnersQuery request,
        CancellationToken cancellationToken)
    {
        // =============================================================
        // Determine current user's roles
        // =============================================================

        var isAdmin =
            currentUser.Roles.Any(role =>
                string.Equals(
                    role,
                    RoleConstants.Admin,
                    StringComparison.OrdinalIgnoreCase));

        var isHallOwner =
            currentUser.Roles.Any(role =>
                string.Equals(
                    role,
                    RoleConstants.HallOwner,
                    StringComparison.OrdinalIgnoreCase));

        // =============================================================
        // Determine access scope
        //
        // Admin:
        //     Can see all Hall Owners.
        //
        // Hall Owner:
        //     Can see only themselves.
        //
        // Admin + Hall Owner:
        //     Admin takes priority, so all Hall Owners are returned.
        // =============================================================

        Guid? hallOwnerId = null;

        if (isHallOwner && !isAdmin)
        {
            if (currentUser.UserId is null)
            {
                return [];
            }

            hallOwnerId = currentUser.UserId.Value;
        }

        // =============================================================
        // Build cache key
        // =============================================================

        var searchText =
            request.SearchText?.Trim() ?? string.Empty;

        var cacheScope =
            hallOwnerId?.ToString() ?? "all";

        var cacheKey =
            $"{CacheKeys.HallOwner}:scope:{cacheScope}:search:{searchText}";

        // =============================================================
        // Check cache
        // =============================================================

        var cachedResponse =
            await cacheService.GetAsync<
                IReadOnlyList<HallOwnerDto>>(
                cacheKey,
                cancellationToken);

        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        // =============================================================
        // Get Hall Owners from database
        // =============================================================

        IReadOnlyList<HallOwnerDto> result;

        if (hallOwnerId.HasValue)
        {
            result =
                await userRepository.GetHallOwnersAsync(
                    searchText,
                    hallOwnerId.Value,
                    cancellationToken);
        }
        else
        {
            result =
                await userRepository.GetHallOwnersAsync(
                    searchText,
                    null,
                    cancellationToken);
        }

        // =============================================================
        // Cache response
        // =============================================================

        await cacheService.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(30),
            cancellationToken);

        // =============================================================
        // Return response
        // =============================================================

        return result;
    }
}