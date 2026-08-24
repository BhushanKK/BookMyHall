using AutoMapper;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class StateProfile : Profile
{
    public StateProfile()
    {
        CreateMap<CreateStateCommand, State>();
        CreateMap<UpdateStateCommand, State>();
        CreateMap<State, StateDto>();
    }
}