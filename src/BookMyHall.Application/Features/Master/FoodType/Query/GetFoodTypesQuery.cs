using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetFoodTypesQuery(PaginationRequest PaginationRequest)
    : IRequest<ApiResponse<PaginatedResult<FoodTypeDto>>>{}