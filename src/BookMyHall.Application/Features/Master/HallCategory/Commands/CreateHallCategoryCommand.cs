using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
namespace BookMyHall.Application.Features.Master;
public sealed class CreateHallCategoryCommand : HallCategoryDto, IRequest<ApiResponse<HallCategoryDto>>;
