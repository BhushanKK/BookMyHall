using AutoMapper;
using BookMyHall.Contracts.Venue;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Mapping;

public sealed class VenueMappingProfile : Profile
{
    public VenueMappingProfile() => CreateMap<HallImage, HallImageDto>();
}