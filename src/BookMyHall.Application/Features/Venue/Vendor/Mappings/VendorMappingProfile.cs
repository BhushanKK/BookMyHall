using AutoMapper;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class VendorMappingProfile : Profile
{
    public VendorMappingProfile()
    {
        CreateMap<CreateVendorCommand, Vendor>()
            .ForMember(destination => destination.LogoUrl, options => options.Ignore())
            .ForMember(
                destination => destination.VendorId,
                options => options.Ignore());

        CreateMap<UpdateVendorCommand, Vendor>()
            .ForMember(destination => destination.LogoUrl, options => options.Ignore())
            .ForMember(
                destination => destination.VendorId,
                options => options.Ignore())
            .ForMember(
                destination => destination.UserId,
                options => options.Ignore())
            .ForMember(
                destination => destination.IsVerified,
                options => options.Ignore())
            .ForMember(
                destination => destination.Rating,
                options => options.Ignore())
            .ForMember(
                destination => destination.ReviewCount,
                options => options.Ignore())
            .ForMember(
                destination => destination.IsDeleted,
                options => options.Ignore());

        CreateMap<Vendor, VendorDto>()
            .ForMember(destination => destination.LogoUrl, options => options.MapFrom(
                source => string.IsNullOrWhiteSpace(source.LogoUrl) ? null : $"/api/vendors/{source.VendorId}/logo/content"));
    }
}
