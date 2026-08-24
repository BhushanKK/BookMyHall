using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateMenuRolePermissionCommandHandler(
    IMenuRolePermissionRepository menuRolePermissionRepository,
    IMenuRepository menuRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<UpdateMenuRolePermissionCommand, ApiResponse<MenuRolePermissionDto>>
{
    public async Task<ApiResponse<MenuRolePermissionDto>> Handle(
        UpdateMenuRolePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await menuRolePermissionRepository.GetByIdAsync(request.MenuRolePermissionId, cancellationToken);

        if (entity is null)
        {
            return ApiResponse<MenuRolePermissionDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.MenuRolePermission),
                HttpStatusCode.NotFound
            );
        }

        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu is null)
        {
            return ApiResponse<MenuRolePermissionDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.Menu),
                HttpStatusCode.NotFound
            );
        }

        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return ApiResponse<MenuRolePermissionDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.Role),
                HttpStatusCode.NotFound
            );
        }

        var existing = await menuRolePermissionRepository.GetByMenuAndRoleAsync(request.MenuId, request.RoleId, cancellationToken);

        if (existing is not null && existing.MenuRolePermissionId != request.MenuRolePermissionId)
        {
            return ApiResponse<MenuRolePermissionDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.MenuRolePermission),
                HttpStatusCode.Conflict
            );
        }

        mapper.Map(request, entity);

        try
        {
            await menuRolePermissionRepository.UpdateAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<MenuRolePermissionDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.MenuRolePermission),
                HttpStatusCode.Conflict
            );
        }

        await cacheService.RemoveAsync(CacheKeys.MenuRolePermissionPaged, cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.MenuRolePermissionPaged}:{request.MenuRolePermissionId}", cancellationToken);

        return ApiResponse<MenuRolePermissionDto>.SuccessResponse
        (
                mapper.Map<MenuRolePermissionDto>(entity),
                messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.MenuRolePermission),
                HttpStatusCode.OK
        );
    }
}