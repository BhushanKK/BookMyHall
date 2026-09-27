using AutoMapper;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;
public sealed class VendorServiceMappingProfile: Profile
{
    public VendorServiceMappingProfile()
    {
        CreateMap<VendorService,VendorServiceDto>();
        CreateMap<CreateVendorServiceCommand,VendorService>();
        CreateMap<UpdateVendorServiceCommand,VendorService>();
    }
}