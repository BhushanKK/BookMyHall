using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateHallBlockCommand : HallBlockDto, IRequest<ApiResponse<HallBlockDto>>;