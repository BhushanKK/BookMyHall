
using AutoMapper;
using BookMyHall.Application.Features.Identity.Authentication;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Application.Features.Identity;

public sealed class AuthenticationMappingProfile : Profile
{
    public AuthenticationMappingProfile() => CreateMap<UserLoginDto, LoginResponse>();
}