using AutoMapper;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class VendorServiceAreaMappingProfile: Profile
{
    public VendorServiceAreaMappingProfile()
    {
        CreateMap<VendorServiceArea,VendorServiceAreaDto>();
    }
}