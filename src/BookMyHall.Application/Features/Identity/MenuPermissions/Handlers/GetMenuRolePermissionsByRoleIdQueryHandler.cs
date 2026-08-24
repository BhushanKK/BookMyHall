using System.Net;

using AutoMapper;
using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetMenuRolePermissionsByRoleIdQueryHandler(
    IMenuRolePermissionRepository menuRolePermissionRepository,
    IRoleRepository roleRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<
        GetMenuRolePermissionsByRoleIdQuery,
        ApiResponse<IReadOnlyList<MenuRolePermissionDto>>>
{
    public async Task<
        ApiResponse<IReadOnlyList<MenuRolePermissionDto>>> Handle(
        GetMenuRolePermissionsByRoleIdQuery request,
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

        var permissions =
            await menuRolePermissionRepository.GetByRoleIdAsync(
                request.RoleId,
                cancellationToken);

        var response =
            mapper.Map<IReadOnlyList<MenuRolePermissionDto>>(
                permissions);

        return ApiResponse<IReadOnlyList<MenuRolePermissionDto>>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.MenuRolePermission),
            HttpStatusCode.OK);
    }
}