using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateHallCommand : HallDto, IRequest<ApiResponse<HallDto>>;