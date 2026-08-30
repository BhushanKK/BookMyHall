using AutoMapper;
using BookMyHall.Application.Features.Identity.Authentication;
using BookMyHall.Application.Features.Identity.Users;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Mapping;

public sealed class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        // ------------------------------------------------------------
        // Signup
        // ------------------------------------------------------------

        CreateMap<SignupUserCommand, User>()
            .ForMember(
                destination => destination.PasswordHash,
                option => option.Ignore())
            .ForMember(
                destination => destination.UserRoles,
                option => option.Ignore());

        // ------------------------------------------------------------
        // Create User - Admin/User Master
        // ------------------------------------------------------------

        CreateMap<CreateUserCommand, User>()
            .ForMember(
                destination => destination.PasswordHash,
                option => option.Ignore())
            .ForMember(
                destination => destination.UserRoles,
                option => option.Ignore());

        // ------------------------------------------------------------
        // User -> UserDto
        // ------------------------------------------------------------

        CreateMap<User, UserDto>()
            .ForMember(
                destination => destination.Roles,
                option => option.MapFrom(
                    source => source.UserRoles
                        .Select(x => x.Role.RoleName)
                        .ToList()));

        // ------------------------------------------------------------
        // Profile Update
        // ------------------------------------------------------------

        CreateMap<ProfileUpdateUserCommand, User>()
            .ForMember(
                destination => destination.PasswordHash,
                option => option.Ignore())
            .ForMember(
                destination => destination.UserRoles,
                option => option.Ignore());

        // ------------------------------------------------------------
        // User -> LoginResponse
        // ------------------------------------------------------------

        CreateMap<User, LoginResponse>()
            .ForMember(
                destination => destination.Roles,
                option => option.MapFrom(
                    source => source.UserRoles
                        .Select(x => x.Role.RoleName)));

        // ------------------------------------------------------------
        // Request -> Command mappings
        // ------------------------------------------------------------

        CreateMap<LoginRequest, LoginCommand>();

        CreateMap<RefreshTokenRequest, RefreshTokenCommand>();

        CreateMap<LogoutRequest, LogoutCommand>();
    }
}