using AutoMapper;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Common.Mapping;

public sealed class MappingHallProfile : Profile
{
    public MappingHallProfile() => CreateMap<Hall, HallDto>().ReverseMap();
}