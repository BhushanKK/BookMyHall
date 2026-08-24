using AutoMapper;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Common.Mapping;

public sealed class MenuRolePermissionMappingProfile : Profile
{
    public MenuRolePermissionMappingProfile()
    {
        CreateMap<MenuRolePermission, MenuRolePermissionDto>().ReverseMap();
    }
}