using AutoMapper;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class DistrictProfile : Profile
{
    public DistrictProfile()
    {
        CreateMap<CreateDistrictCommand, District>();

        CreateMap<UpdateDistrictCommand, District>();

        CreateMap<District, DistrictDto>();
    }
}