using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;
public sealed record DeleteHallImageCommand(Guid HallImageId)
    : IRequest<ApiResponse<bool>>;