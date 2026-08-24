using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetMenuRolePermissionByIdQueryHandler(
    IMenuRolePermissionRepository menuRolePermissionRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        GetMenuRolePermissionByIdQuery,
        ApiResponse<MenuRolePermissionDto>>
{
    public async Task<ApiResponse<MenuRolePermissionDto>> Handle(GetMenuRolePermissionByIdQuery request,CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.MenuRolePermission}:{request.MenuRolePermissionId}";

        var cached = await cacheService.GetAsync<MenuRolePermissionDto>(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return ApiResponse<MenuRolePermissionDto>.SuccessResponse
            (
                cached,
                string.Empty,
                HttpStatusCode.OK
            );
        }

        var entity = await menuRolePermissionRepository.GetByIdAsync(request.MenuRolePermissionId, cancellationToken);

        if (entity is null)
        {
            return ApiResponse<MenuRolePermissionDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.MenuRolePermission),
                HttpStatusCode.NotFound
            );
        }

        var response = mapper.Map<MenuRolePermissionDto>(entity);

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<MenuRolePermissionDto>.SuccessResponse
        (
            response,
            string.Empty,
            HttpStatusCode.OK
        );
    }
}