using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Master;
public sealed record CreateFoodTypeCommand(string FoodTypeName)
    : IRequest<ApiResponse<Guid>>;