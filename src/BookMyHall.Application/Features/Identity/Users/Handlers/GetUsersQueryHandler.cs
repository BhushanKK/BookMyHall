using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class GetUsersQueryHandler(IUserRepository userRepository,IMessageHelper messageHelper,
    ICacheService cacheService,IR2StorageService storageService)
    : IRequestHandler<GetUsersQuery, ApiResponse<PaginatedResponse<UserDto>>>
{
    private static readonly TimeSpan CacheDuration =TimeSpan.FromMinutes(30);

    private static readonly TimeSpan PreSignedUrlDuration =TimeSpan.FromDays(7);

    public async Task<ApiResponse<PaginatedResponse<UserDto>>> Handle(GetUsersQuery request,CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;
        // ------------------------------------------------------------
        // 1. Build the paginated cache key
        // ------------------------------------------------------------

        var cacheKey =
            CacheKeyBuilder.BuildPaginatedKey<UserDto>(
                CacheKeys.UsersPaged,
                pagination.PageNumber,
                pagination.PageSize,
                pagination.SearchText,
                pagination.SortBy,
                pagination.SortDescending);

        // ------------------------------------------------------------
        // 2. Check cache first
        // ------------------------------------------------------------

        var cachedResponse =await cacheService.GetAsync<PaginatedResponse<UserDto>>(cacheKey,cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<UserDto>>.SuccessResponse(cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.User),HttpStatusCode.OK);
        }

        // ------------------------------------------------------------
        // 3. Get users from database
        // ------------------------------------------------------------

        var pagedResult =await userRepository.GetAllAsync(pagination,cancellationToken);
        var users = pagedResult.Items;

        // ------------------------------------------------------------
        // 5. Generate R2 pre-signed URLs in parallel
        // ------------------------------------------------------------
        //
        // Only users having a profile image are processed.
        //
        // Task.WhenAll prevents sequential R2 URL generation.
        // ------------------------------------------------------------

        var usersWithImages = users
            .Where(user =>!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
            .ToList();

        if (usersWithImages.Count > 0)
        {
            await Task.WhenAll(
                usersWithImages.Select(async user =>
                {
                    user.ProfileImageUrl =await storageService.GetPreSignedUrlAsync(user.ProfileImageUrl!,
                            PreSignedUrlDuration,
                            cancellationToken);
                }));
        }

        // ------------------------------------------------------------
        // 6. Build response
        // ------------------------------------------------------------

        var response = new PaginatedResponse<UserDto>
        {
            Items = users,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        // ------------------------------------------------------------
        // 7. Cache final response
        // ------------------------------------------------------------
        //
        // The cached object contains the pre-signed URLs.
        //
        // Cache = 30 minutes
        // URL     = 7 days
        //
        // Therefore the cache will never retain a URL beyond the
        // URL's configured lifetime.
        // ------------------------------------------------------------

        await cacheService.SetAsync(cacheKey,response,CacheDuration,cancellationToken);

        // ------------------------------------------------------------
        // 8. Return response
        // ------------------------------------------------------------

        return ApiResponse<PaginatedResponse<UserDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.User),HttpStatusCode.OK);
    }
}