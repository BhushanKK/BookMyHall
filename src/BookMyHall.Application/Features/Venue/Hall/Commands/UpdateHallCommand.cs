using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateHallCommand : HallDto, IRequest<ApiResponse<HallDto>>;