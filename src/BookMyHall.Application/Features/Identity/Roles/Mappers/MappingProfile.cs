using AutoMapper;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Common.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile() => CreateMap<Role, RoleDto>().ReverseMap();
}