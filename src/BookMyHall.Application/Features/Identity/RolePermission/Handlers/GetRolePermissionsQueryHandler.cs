using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetRolePermissionsQueryHandler(
    IRolePermissionRepository rolePermissionRepository,
    IMapper mapper,IMessageHelper messageHelper)
    : IRequestHandler<GetRolePermissionsQuery,
        ApiResponse<IReadOnlyList<RolePermissionDto>>>
{
    public async Task<ApiResponse<IReadOnlyList<RolePermissionDto>>> Handle(
        GetRolePermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var rolePermissions =await rolePermissionRepository.GetByRoleIdAsync(request.RoleId,cancellationToken);
        if (rolePermissions.Count == 0)
        {
            return ApiResponse<IReadOnlyList<RolePermissionDto>>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities,
                    EntityKeys.RolePermission),HttpStatusCode.NotFound);
        }

        var response = mapper.Map<IReadOnlyList<RolePermissionDto>>(rolePermissions);
        return ApiResponse<IReadOnlyList<RolePermissionDto>>.SuccessResponse( response,string.Empty,HttpStatusCode.OK);
    }
}