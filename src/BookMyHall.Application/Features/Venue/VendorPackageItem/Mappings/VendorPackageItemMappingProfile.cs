using AutoMapper;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;
public sealed class VendorPackageItemMappingProfile: Profile
{
    public VendorPackageItemMappingProfile()
    {
        CreateMap<VendorPackageItem,VendorPackageItemDto>();
    }
}