using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class RemoveRolePermissionCommandHandler(
    IRolePermissionRepository rolePermissionRepository,
    IUnitOfWork unitOfWork,IMessageHelper messageHelper)
    : IRequestHandler< RemoveRolePermissionCommand,ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle( RemoveRolePermissionCommand request,CancellationToken cancellationToken)
    {
var rolePermission =await rolePermissionRepository.GetAsync(
                request.RoleId,request.PermissionId,cancellationToken);

        if (rolePermission is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities,
                    EntityKeys.RolePermission), HttpStatusCode.NotFound);
        }

        rolePermissionRepository.Delete(rolePermission);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse( true,
            messageHelper.DeletedEntity(ResourceNames.Entities,
                EntityKeys.RolePermission), HttpStatusCode.OK);
    }
}