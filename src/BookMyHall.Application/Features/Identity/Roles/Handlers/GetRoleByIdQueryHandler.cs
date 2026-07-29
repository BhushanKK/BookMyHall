using MediatR;
using AutoMapper;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetRoleByIdQueryHandler(
    IRoleRepository roleRepository,
    IMapper mapper)
    : IRequestHandler<GetRoleByIdQuery, ApiResponse<RoleDto>>
{
    public async Task<ApiResponse<RoleDto>> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.RoleId,cancellationToken);

        if (role is null)
        {
            return ApiResponse<RoleDto>.Failure
            (
                string.Format(ApiMessages.RecordNotFound,Entities.Role)
            );
        }

        return ApiResponse<RoleDto>.Success
        (
            mapper.Map<RoleDto>(role),
            string.Format(ApiMessages.RecordRetrieved,Entities.Role),
            request.RoleId
        );
    }
}