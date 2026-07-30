using AutoMapper;
using BookMyHall.Application.Features.Identity.Users;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Mapping;

public sealed class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        CreateMap<CreateUserCommand, User>()
            .ForMember(
                destination => destination.PasswordHash,
                option => option.Ignore())
            .ForMember(
                destination => destination.UserRoles,
                option => option.Ignore());

        CreateMap<User, UserDto>();
    }
}