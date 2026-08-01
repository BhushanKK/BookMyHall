using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetFoodTypeByIdQuery(Guid FoodTypeId)
    : IRequest<ApiResponse<FoodTypeDto>>;