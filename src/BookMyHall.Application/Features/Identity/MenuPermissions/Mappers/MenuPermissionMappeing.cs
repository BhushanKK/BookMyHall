using AutoMapper;

using BookMyHall.Application.Features.Identity;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Common.Mapping;

public sealed class MenuPermissionMappingProfile : Profile
{
    public MenuPermissionMappingProfile()
    {
        CreateMap<MenuPermission, MenuPermissionDto>().ReverseMap();

        CreateMap<CreateMenuPermissionCommand, MenuPermission>();

        CreateMap<UpdateMenuPermissionCommand, MenuPermission>();
    }
}