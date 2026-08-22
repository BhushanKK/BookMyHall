using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetHallBlockByIdQuery(Guid HallBlockId): IRequest<ApiResponse<HallBlock>>;