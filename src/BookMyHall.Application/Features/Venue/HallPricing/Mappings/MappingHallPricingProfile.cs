using AutoMapper;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Common.Mapping;

public sealed class MappingHallPricingProfile : Profile
{
    public MappingHallPricingProfile() => CreateMap<HallPricing, HallPricingDto>().ReverseMap();
}