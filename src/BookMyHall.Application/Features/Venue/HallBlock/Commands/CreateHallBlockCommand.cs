using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateHallBlockCommand:HallBlockDto, IRequest<ApiResponse<HallBlockDto>>;
