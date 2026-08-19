using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class DeletePermissionCommandHandler(
    IPermissionRepository permissionRepository,
    IUnitOfWork unitOfWork,IMessageHelper messageHelper)
    : IRequestHandler<DeletePermissionCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeletePermissionCommand request,CancellationToken cancellationToken)
    {
        var permission = await permissionRepository.GetByIdAsync(request.PermissionId,cancellationToken);

        if (permission is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.Permission),HttpStatusCode.NotFound);
        }

        permission.Deactivate();
        await permissionRepository.UpdateAsync(permission,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true,messageHelper.DeletedEntity(
                ResourceNames.Entities,EntityKeys.Permission),HttpStatusCode.OK);
    }
}