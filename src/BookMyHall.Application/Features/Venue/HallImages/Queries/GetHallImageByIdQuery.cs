using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetHallImageByIdQuery(Guid HallImageId)
    : IRequest<ApiResponse<HallImageDto>>;