using AutoMapper;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Common.Mapping;

public sealed class MenuMappingProfile : Profile
{
    public MenuMappingProfile() => 
    CreateMap<Menu, MenuDto> ().ReverseMap();
}