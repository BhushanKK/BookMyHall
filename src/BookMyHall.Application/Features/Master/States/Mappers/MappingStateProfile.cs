using AutoMapper;
using BookMyHall.Application.Features.Master;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Common.Mapping;

public sealed class MappingStateProfile : Profile
{
    public MappingStateProfile() => CreateMap<State, StateDto>().ReverseMap();
}