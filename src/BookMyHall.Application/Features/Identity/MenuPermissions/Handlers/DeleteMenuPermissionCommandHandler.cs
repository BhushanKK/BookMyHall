using MediatR;
using System.Net;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class DeleteMenuPermissionCommandHandler(
    IMenuPermissionRepository menuPermissionRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper)
    : IRequestHandler<DeleteMenuPermissionCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteMenuPermissionCommand request,
        CancellationToken cancellationToken)
    {
        var menuPermission = await menuPermissionRepository.GetAsync(
            request.MenuId,
            request.PermissionId,
            cancellationToken);

        if (menuPermission is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.MenuPermission),
                HttpStatusCode.NotFound
            );
        }

        await menuPermissionRepository.UpdateAsync(
            menuPermission,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(
            true,
            messageHelper.DeletedEntity(
                ResourceNames.Entities,
                EntityKeys.MenuPermission),
            HttpStatusCode.OK
        );
    }
}