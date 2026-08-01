using AutoMapper;
using BookMyHall.Domain.Masters;
namespace BookMyHall.Application.Features.Master;
public sealed class FoodTypeProfile : Profile
{
    public FoodTypeProfile()
    {
        CreateMap<CreateFoodTypeCommand, FoodType>();

        CreateMap<UpdateFoodTypeCommand, FoodType>();

        CreateMap<FoodType, FoodTypeDto>();
    }
}