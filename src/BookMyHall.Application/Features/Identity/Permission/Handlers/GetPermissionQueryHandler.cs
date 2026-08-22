using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetPermissionQueryHandler(
    IPermissionRepository permissionRepository,
    IMapper mapper,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetPermissionQuery,ApiResponse<PaginatedResponse<Permission>>>
{
    public async Task<ApiResponse<PaginatedResponse<Permission>>> Handle(
        GetPermissionQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;

        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<Permission>(
            CacheKeys.Permissions,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<Permission>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<Permission>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Permission),
                HttpStatusCode.OK
            );
        }
        var pagedResult = await permissionRepository.GetAllAsync(request.paginationRequest,cancellationToken);

        var response = new PaginatedResponse<Permission>
        {
            Items = mapper.Map<IReadOnlyList<Permission>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };
        
       await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<PaginatedResponse<Permission>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities, EntityKeys.Permission), HttpStatusCode.OK);
    }
}