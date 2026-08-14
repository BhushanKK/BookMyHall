using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetHallBlockByIdQuery(Guid HallBlockId): IRequest<ApiResponse<HallBlockDto>>;