using AutoMapper;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class CountryProfile : Profile
{
    public CountryProfile()
    {
        CreateMap<CreateCountryCommand, Country>();
        CreateMap<UpdateCountryCommand, Country>();
        CreateMap<Country, CountryDto>();
    }
}