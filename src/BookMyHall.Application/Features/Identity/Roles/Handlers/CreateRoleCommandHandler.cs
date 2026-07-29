using MediatR;
using AutoMapper;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Constants;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreateRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<CreateRoleCommand, ApiResponse<RoleDto>>
{
    public async Task<ApiResponse<RoleDto>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = mapper.Map<Role>(request);

        role.CreatedDate = DateTimeOffset.UtcNow;
        role.CreatedBy= new Guid(); //later on need to change.

        await roleRepository.AddAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<RoleDto>.Success
        (
            mapper.Map<RoleDto>(role),
            string.Format(ApiMessages.RecordCreated,Entities.Role),
            request.RoleId
        );
    }
}