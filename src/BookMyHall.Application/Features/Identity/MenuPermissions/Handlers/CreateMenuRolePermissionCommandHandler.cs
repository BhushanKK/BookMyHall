using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreateMenuRolePermissionCommandHandler(
    IMenuRolePermissionRepository menuRolePermissionRepository,
    IMenuRepository menuRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        CreateMenuRolePermissionCommand,
        ApiResponse<MenuRolePermissionDto>>
{
    public async Task<ApiResponse<MenuRolePermissionDto>> Handle(
        CreateMenuRolePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu is null)
        {
            return ApiResponse<MenuRolePermissionDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.Menu),
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

        var existing = await menuRolePermissionRepository.GetByMenuAndRoleAsync
        (
            request.MenuId,
            request.RoleId,
            cancellationToken
        );

        if (existing is not null)
        {
            return ApiResponse<MenuRolePermissionDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.MenuRolePermission),
                HttpStatusCode.Conflict
            );
        }

        var entity = mapper.Map<MenuRolePermission>(request);

        entity.MenuRolePermissionId = Guid.NewGuid();

        try
        {
            await menuRolePermissionRepository.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<MenuRolePermissionDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.MenuRolePermission),
                HttpStatusCode.Conflict
            );
        }

        await cacheService.RemoveByPrefixAsync(CacheKeys.MenuRolePermissionPaged, cancellationToken);

        return ApiResponse<MenuRolePermissionDto>.SuccessResponse
        (
            mapper.Map<MenuRolePermissionDto>(entity),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.MenuRolePermission),
            HttpStatusCode.Created
        );
    }
}