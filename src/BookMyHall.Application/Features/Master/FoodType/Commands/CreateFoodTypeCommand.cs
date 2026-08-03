using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Master;
public sealed class CreateFoodTypeCommand
    :FoodTypeDto, IRequest<ApiResponse<FoodTypeDto>>;