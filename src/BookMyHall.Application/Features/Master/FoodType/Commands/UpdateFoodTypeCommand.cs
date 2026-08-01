using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateFoodTypeCommand()
    :FoodTypeDto, IRequest<ApiResponse<FoodTypeDto>>;
