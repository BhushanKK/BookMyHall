using AutoMapper;
using BookMyHall.Domain.Masters;
namespace BookMyHall.Application.Features.Master;

public sealed class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<CreateServiceCommand, Service>();

        CreateMap<UpdateServiceCommand, Service>();

        CreateMap<Service, ServiceDto>();
    }
}