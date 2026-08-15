
using AutoMapper;
using BookMyHall.Application.Features.Identity.Authentication;

namespace BookMyHall.Application.Features.Identity;

public sealed class AuthenticationMappingProfile : Profile
{
    public AuthenticationMappingProfile() => CreateMap<UserLoginDto, LoginResponse>();
}