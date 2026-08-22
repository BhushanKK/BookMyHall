using MediatR;
using System.Net;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity;

public sealed class DeleteRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<DeleteRoleCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.Role),
                HttpStatusCode.NotFound
            );
        }

        role.Deactivate();

        await roleRepository.UpdateAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.RemoveAsync($"{CacheKeys.Roles}:{request.RoleId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.Roles}:page:", cancellationToken);

        return ApiResponse<bool>.SuccessResponse
        (
            true,messageHelper.DeletedEntity(ResourceNames.Entities,EntityKeys.Role),
            HttpStatusCode.OK
        );
    }
}