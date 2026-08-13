using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;
public sealed record DeleteHallCategoryCommand(Guid HallCategoryId): IRequest<ApiResponse<bool>>;