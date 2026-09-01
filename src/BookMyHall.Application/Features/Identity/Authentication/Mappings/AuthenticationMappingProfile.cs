using AutoMapper;
using BookMyHall.Application.Features.Identity.Authentication;
using BookMyHall.Application.Features.Authentication.Commands.SetPassword;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Application.Features.Identity;

public sealed class AuthenticationMappingProfile : Profile
{
    public AuthenticationMappingProfile()
    {
        CreateMap<UserLoginDto, LoginResponse>();
        CreateMap<SetPasswordRequest, SetPasswordCommand>();
    }
}