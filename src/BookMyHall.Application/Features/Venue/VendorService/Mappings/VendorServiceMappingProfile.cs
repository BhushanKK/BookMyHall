using AutoMapper;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;
public sealed class VendorServiceMappingProfile: Profile
{
    public VendorServiceMappingProfile()
    {
        CreateMap<VendorService,VendorServiceDto>()
            .ForMember(x => x.BusinessName, options => options.MapFrom(x => x.Vendor.BusinessName))
            .ForMember(x => x.VendorCategoryName, options => options.MapFrom(x => x.VendorSubCategory.VendorCategory.VendorCategoryName))
            .ForMember(x => x.VendorSubCategoryName, options => options.MapFrom(x => x.VendorSubCategory.VendorSubCategoryName));
        CreateMap<CreateVendorServiceCommand,VendorService>();
        CreateMap<UpdateVendorServiceCommand,VendorService>();
    }
}
