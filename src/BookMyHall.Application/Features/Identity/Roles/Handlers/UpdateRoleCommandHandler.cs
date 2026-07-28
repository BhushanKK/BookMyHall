using MediatR;
using AutoMapper;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<UpdateRoleCommand, ApiResponse<RoleDto>>
{
    public async Task<ApiResponse<RoleDto>> Handle(
        UpdateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return ApiResponse<RoleDto>.Failure
            (
                string.Format(ApiMessages.RecordNotFound, Entities.Role)
            );
        }

        // Map request values into existing entity
        mapper.Map(request, role);
        role.UpdatedDate = DateTimeOffset.UtcNow;
        role.UpdatedBy= new Guid(); //later on need to change.

        await roleRepository.UpdateAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<RoleDto>.Success
        (
            mapper.Map<RoleDto>(role),
            string.Format(ApiMessages.RecordUpdated, Entities.Role),
            request.RoleId
        );
    }
}