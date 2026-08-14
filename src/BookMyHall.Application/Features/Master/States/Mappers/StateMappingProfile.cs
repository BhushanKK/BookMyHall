using AutoMapper;
using BookMyHall.Application.Features.Master;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Common.Mapping;

public sealed class StateMappingProfile : Profile
{
      public StateMappingProfile()
    {
        CreateMap<State, StateDto>();

        CreateMap<CreateStateCommand, State>()
            .ForMember(dest => dest.StateId,opt => opt.Ignore())
            .ForMember(dest => dest.IsActive,opt => opt.Ignore());

        CreateMap<UpdateStateCommand, State>()
            .ForMember(dest => dest.StateId,opt => opt.Ignore())
            .ForMember(dest => dest.IsActive,opt => opt.Ignore());
    }
}