using MediatR;
using System.Net;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class DeleteRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork)
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
                string.Format(ApiMessages.RecordNotFound, Entities.Role),
                HttpStatusCode.NotFound
            );
        }

        role.Deactivate();
        await roleRepository.UpdateAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse
        (
            true,
            string.Format(ApiMessages.RecordDeleted, Entities.Role),
            HttpStatusCode.OK
        );
    }
}