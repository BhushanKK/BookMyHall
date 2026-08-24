using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetMenuRolePermissionQueryHandler(
    IMenuRolePermissionRepository menuRolePermissionRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        GetMenuRolePermissionQuery,
        ApiResponse<PaginatedResponse<MenuRolePermissionDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<MenuRolePermissionDto>>> Handle(
        GetMenuRolePermissionQuery request,
        CancellationToken cancellationToken)
    {
        var paginationRequest = request.PaginationRequest;

        var cacheKey =
            $"{CacheKeys.MenuRolePermissionPaged}" +
            $":{paginationRequest.PageNumber}" +
            $":{paginationRequest.PageSize}" +
            $":{paginationRequest.SearchText}" +
            $":{paginationRequest.SortBy}" +
            $":{paginationRequest.SortDescending}";

        var cached = await cacheService.GetAsync<PaginatedResponse<MenuRolePermissionDto>>(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return ApiResponse<PaginatedResponse<MenuRolePermissionDto>>.SuccessResponse
            (
                cached,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.MenuRolePermission),
                HttpStatusCode.OK
            );
        }

        var result = await menuRolePermissionRepository.GetAllAsync(paginationRequest, cancellationToken);

        var response = new PaginatedResponse<MenuRolePermissionDto>
        {
            Items = mapper.Map<IReadOnlyList<MenuRolePermissionDto>>(result.Items),
            PageNumber = paginationRequest.PageNumber,
            PageSize = paginationRequest.PageSize,
            TotalRecords = result.TotalCount
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<PaginatedResponse<MenuRolePermissionDto>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.MenuRolePermission),
            HttpStatusCode.OK
        );
    }
}