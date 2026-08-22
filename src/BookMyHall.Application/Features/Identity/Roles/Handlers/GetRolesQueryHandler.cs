using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetRolesQueryHandler(
    IRoleRepository roleRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<GetRolesQuery, ApiResponse<PaginatedResponse<Role>>>
{
    public async Task<ApiResponse<PaginatedResponse<Role>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;

        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<Role>(
            CacheKeys.Roles,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<Role>>(cacheKey,cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<Role>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Role),
                HttpStatusCode.OK
            );
        }

        var pagedResult = await roleRepository.GetAllAsync(pagination, cancellationToken);

        var response = new PaginatedResponse<Role>
        {
            Items = mapper.Map<IReadOnlyList<Role>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<PaginatedResponse<Role>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Role),
            HttpStatusCode.OK
        );
    }
}