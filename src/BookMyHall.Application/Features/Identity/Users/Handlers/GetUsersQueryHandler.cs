using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class GetUsersQueryHandler(
    IUserRepository userRepository,
    IMapper mapper,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetUsersQuery, ApiResponse<PaginatedResponse<UserDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<UserDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
          var pagination = request.paginationRequest;

        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<User>(
            CacheKeys.Users,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<UserDto>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<UserDto>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.User),
                HttpStatusCode.OK
            );
        }
        var pagedResult = await userRepository.GetAllAsync(request.paginationRequest, cancellationToken);

        var response = new PaginatedResponse<UserDto>
        {
            Items = mapper.Map<IReadOnlyList<UserDto>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        
        return ApiResponse<PaginatedResponse<UserDto>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.OK
        );
    }
}