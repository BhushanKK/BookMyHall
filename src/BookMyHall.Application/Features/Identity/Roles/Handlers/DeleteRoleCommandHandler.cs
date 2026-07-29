using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Constants;

public sealed class DeleteRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteRoleCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.RoleId,cancellationToken);

        if (role is null)
        {
            return ApiResponse<bool>.Failure
            (
                string.Format(ApiMessages.RecordNotFound,Entities.Role)
            );
        }

        role.Deactivate();

        await roleRepository.UpdateAsync(role,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Success
        (
            true,
            string.Format(ApiMessages.RecordDeleted,Entities.Role),
            request.RoleId
        );
    }
}