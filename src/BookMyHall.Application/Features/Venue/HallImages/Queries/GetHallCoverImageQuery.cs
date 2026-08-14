using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;

namespace BookMyHall.Application.Features.Venue;
public sealed record GetHallCoverImageQuery(Guid HallId)
    : IRequest<ApiResponse<HallImageDto>>;