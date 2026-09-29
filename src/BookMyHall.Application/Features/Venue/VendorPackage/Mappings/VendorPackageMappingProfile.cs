using AutoMapper;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public class VendorPackageMappingProfile : Profile
{
    public VendorPackageMappingProfile()
    {
        CreateMap<VendorPackage, VendorPackageDto>().ReverseMap();
        CreateMap<CreateVendorPackageCommand, VendorPackage>();
        CreateMap<UpdateVendorPackageCommand, VendorPackage>();
    }
}
