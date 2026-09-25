using AutoMapper;
using BookMyHall.Domain.Venue;
namespace BookMyHall.Application.Features.Venue;
public sealed class VendorMappingProfile : Profile
{
    public VendorMappingProfile()
    {
        CreateMap<CreateVendorCommand, Vendors>();
        CreateMap<UpdateVendorCommand, Vendors>();
        CreateMap<Vendors, VendorDto>();
    }
}