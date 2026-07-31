using AutoMapper;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class AreaMappingProfile : Profile
{
    public AreaMappingProfile()
    {
        CreateMap<CreateAreaCommand, Area>();

        CreateMap<UpdateAreaCommand, Area>();

        CreateMap<Area, AreaDto>();
    }
}