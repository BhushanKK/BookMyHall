using AutoMapper;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;
public sealed class VendorSubCategoryMappingProfile: Profile
{
    public VendorSubCategoryMappingProfile()
    {
        CreateMap<VendorSubCategory,VendorSubCategoryDto>();
        CreateMap<CreateVendorSubCategoryCommand,VendorSubCategory>();
        CreateMap<UpdateVendorSubCategoryCommand,VendorSubCategory>();
    }
}