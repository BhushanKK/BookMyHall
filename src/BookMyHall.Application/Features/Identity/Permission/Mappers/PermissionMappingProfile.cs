using AutoMapper;

using BookMyHall.Domain.Identity;
namespace BookMyHall.Application.Features.Identity;

public sealed class PermissionMappingProfile : Profile
{
    public PermissionMappingProfile()
    {
        CreateMap<Permission, PermissionDto>();

        CreateMap<CreatePermissionCommand, Permission>()
            .ForMember(destination => destination.PermissionId,options => options.Ignore());

        CreateMap<UpdatePermissionCommand, Permission>()
            .ForMember(destination => destination.PermissionId,options => options.Ignore());
    }
}