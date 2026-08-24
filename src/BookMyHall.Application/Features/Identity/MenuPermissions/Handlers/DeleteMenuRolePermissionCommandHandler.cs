using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class DeleteMenuRolePermissionCommandHandler(
    IMenuRolePermissionRepository menuRolePermissionRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<DeleteMenuRolePermissionCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteMenuRolePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await menuRolePermissionRepository.GetByIdAsync(request.MenuRolePermissionId,cancellationToken);

        if (entity is null)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.MenuRolePermission),
                HttpStatusCode.NotFound
            );
        }

        await menuRolePermissionRepository.DeleteAsync(entity, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await cacheService.RemoveAsync(CacheKeys.MenuRolePermissionPaged, cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.MenuRolePermissionPaged}:{request.MenuRolePermissionId}", cancellationToken);

        return ApiResponse<bool>.SuccessResponse
        (
            true,
            messageHelper.DeletedEntity(ResourceNames.Entities,EntityKeys.MenuRolePermission),
            HttpStatusCode.OK
        );
    }
}