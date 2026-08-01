using AutoMapper;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class FacilityProfile : Profile
{
    public FacilityProfile()
    {
        CreateMap<CreateFacilityCommand, Facility>();

        CreateMap<UpdateFacilityCommand, Facility>();

        CreateMap<Facility, FacilityDto>();
    }
}