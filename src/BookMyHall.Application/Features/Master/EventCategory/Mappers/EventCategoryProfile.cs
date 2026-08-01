using AutoMapper;
using BookMyHall.Domain.Masters;
namespace BookMyHall.Application.Features.Master;
public sealed class EventCategoryProfile : Profile
{
    public EventCategoryProfile()
    {
        CreateMap<CreateEventCategoryCommand, EventCategory>();

        CreateMap<UpdateEventCategoryCommand, EventCategory>();

        CreateMap<EventCategory, EventCategoryDto>();
    }
}