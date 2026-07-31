using AutoMapper;

using BookMyHall.Application.Features.Identity.Authentication;
using BookMyHall.Application.Features.Identity.Users;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Mapping;

public sealed class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        CreateMap<CreateUserCommand, User>()
            .ForMember(destination => destination.PasswordHash,
                option => option.Ignore())
            .ForMember(destination => destination.UserRoles,
                option => option.Ignore());

        CreateMap<User, UserDto>()
            .ForMember(destination => destination.Roles,
                option => option.MapFrom(source => source.UserRoles
            .Select(x => x.Role.RoleName)
            .ToList()));
        
        CreateMap<UpdateUserCommand, User>()
            .ForMember(destination => destination.PasswordHash, option => option.Ignore())
            .ForMember(destination => destination.UserRoles, option => option.Ignore());

        CreateMap<CreateUserCommand, User>()
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.UserRoles, o => o.Ignore());
        
        CreateMap<User, LoginResponse>()
        .ForMember(
        destination => destination.Roles,
        option => option.MapFrom(source => source.UserRoles.Select(x => x.Role.RoleName)));
        
        CreateMap<User, UserDto>();
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<RefreshTokenRequest, RefreshTokenCommand>();
        CreateMap<LogoutRequest, LogoutCommand>();
    }
}