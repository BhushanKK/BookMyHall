using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpsertMenuRolePermissionCommandHandler(
    IMenuRolePermissionRepository menuRolePermissionRepository,
    IMenuRepository menuRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<
        UpsertMenuRolePermissionCommand,
        ApiResponse<IReadOnlyList<MenuRolePermissionDto>>>
{
    public async Task<
        ApiResponse<IReadOnlyList<MenuRolePermissionDto>>> Handle(
        UpsertMenuRolePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var role =
            await roleRepository.GetByIdAsync(
                request.RoleId,
                cancellationToken);

        if (role is null)
        {
            return ApiResponse<
                IReadOnlyList<MenuRolePermissionDto>
            >.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.Role),
                HttpStatusCode.NotFound);
        }

        if (request.Permissions.Count == 0)
        {
            return ApiResponse<
                IReadOnlyList<MenuRolePermissionDto>
            >.SuccessResponse(
                [],
                messageHelper.UpdatedEntity(
                    ResourceNames.Entities,
                    EntityKeys.MenuRolePermission),
                HttpStatusCode.OK);
        }

        var menuIds =
            request.Permissions
                .Select(x => x.MenuId)
                .Distinct()
                .ToList();

        foreach (var menuId in menuIds)
        {
            var menu =
                await menuRepository.GetByIdAsync(
                    menuId,
                    cancellationToken);

            if (menu is null)
            {
                return ApiResponse<
                    IReadOnlyList<MenuRolePermissionDto>
                >.FailureResponse(
                    messageHelper.NotFoundEntity(
                        ResourceNames.Entities,
                        EntityKeys.Menu),
                    HttpStatusCode.NotFound);
            }
        }

        var entities =
            request.Permissions
                .Select(permission =>
                {
                    var entity =
                        mapper.Map<MenuRolePermission>(
                            permission);

                    entity.MenuRolePermissionId =
                        Guid.NewGuid();

                    entity.RoleId =
                        request.RoleId;

                    return entity;
                })
                .ToList();

        try
        {
            await menuRolePermissionRepository.UpsertRangeAsync(
                entities,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<
                IReadOnlyList<MenuRolePermissionDto>
            >.FailureResponse(
                messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,
                    EntityKeys.MenuRolePermission),
                HttpStatusCode.Conflict);
        }

        var response =
            mapper.Map<IReadOnlyList<MenuRolePermissionDto>>(
                entities);

        return ApiResponse<
            IReadOnlyList<MenuRolePermissionDto>
        >.SuccessResponse(
            response,
            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.MenuRolePermission),
            HttpStatusCode.OK);
    }
}