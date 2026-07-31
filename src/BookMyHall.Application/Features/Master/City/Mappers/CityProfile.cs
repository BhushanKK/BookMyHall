using AutoMapper;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<CreateCityCommand, City>();

        CreateMap<UpdateCityCommand, City>();

        CreateMap<City, CityDto>();
    }
}