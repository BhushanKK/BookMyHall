using AutoMapper;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Mapping;
public sealed class VendorAvailabilityProfile: Profile
{
    public VendorAvailabilityProfile()
    {
        CreateMap<VendorAvailability,VendorAvailabilityDto>();
    }
}