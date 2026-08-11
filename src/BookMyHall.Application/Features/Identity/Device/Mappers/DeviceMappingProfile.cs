using AutoMapper;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed class DeviceMappingProfile : Profile
{
    public DeviceMappingProfile()
    {
        CreateMap<Device, DeviceDto>();

        CreateMap<RegisterDeviceCommand, Device>();

        CreateMap<UpdateDeviceCommand, Device>()
            .ForMember(dest => dest.DeviceId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<DeviceDto, Device>();
    }
}