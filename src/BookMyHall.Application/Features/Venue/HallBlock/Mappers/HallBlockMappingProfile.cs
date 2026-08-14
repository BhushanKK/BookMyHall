using AutoMapper;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class HallBlockMappingProfile : Profile
{
    public HallBlockMappingProfile()
    {
        CreateMap<HallBlock, HallBlockDto>();

        CreateMap<CreateHallBlockCommand, HallBlock>()
            .ForMember(dest => dest.HallBlockId,opt => opt.Ignore())
            .ForMember(dest => dest.IsActive,opt => opt.Ignore());

        CreateMap<UpdateHallBlockCommand, HallBlock>()
            .ForMember(dest => dest.HallBlockId,opt => opt.Ignore())
            .ForMember(dest => dest.IsActive,opt => opt.Ignore());
    }
}