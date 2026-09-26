using AutoMapper;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;
public sealed class VendorCategoryProfile : Profile
{
    public VendorCategoryProfile()
    {
        CreateMap<CreateVendorCategoryCommand, VendorCategory>();
        CreateMap<UpdateVendorCategoryCommand, VendorCategory>();
        CreateMap<VendorCategory, VendorCategoryDto>();
    }
}