using AutoMapper;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class CancellationPolicyMappingProfile : Profile
{
    public CancellationPolicyMappingProfile()
    {
        CreateMap<CreateCancellationPolicyCommand, CancellationPolicy>();
        CreateMap<UpdateCancellationPolicyCommand, CancellationPolicy>();
        CreateMap<CancellationPolicy, CancellationPolicyDto>();
    }
}