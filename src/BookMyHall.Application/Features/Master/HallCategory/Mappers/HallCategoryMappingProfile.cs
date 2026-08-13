using AutoMapper;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class HallCategoryMappingProfile : Profile
{
    public HallCategoryMappingProfile()
    {
        CreateMap<HallCategory, HallCategoryDto>();

        CreateMap<CreateHallCategoryCommand, HallCategory>();

        CreateMap<UpdateHallCategoryCommand, HallCategory>()
            .ForMember(
                dest => dest.HallCategoryId,
                opt => opt.Ignore());
    }
}