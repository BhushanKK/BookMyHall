using AutoMapper;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed class RolePermissionMappingProfile : Profile
{
    public RolePermissionMappingProfile()
    {
        CreateMap<RolePermission, RolePermissionDto>();

        CreateMap<AssignRolePermissionCommand, RolePermission>();
    }
}