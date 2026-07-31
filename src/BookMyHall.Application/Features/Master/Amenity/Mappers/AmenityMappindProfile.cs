using AutoMapper;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class AmenityProfile : Profile
{
    public AmenityProfile()
    {
        CreateMap<CreateAmenityCommand, Amenity>();

        CreateMap<Amenity, AmenityDto>();
    }
}