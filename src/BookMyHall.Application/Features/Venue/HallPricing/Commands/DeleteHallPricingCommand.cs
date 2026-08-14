using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed record DeleteHallPricingCommand(Guid HallPricingId)
    : IRequest<ApiResponse<bool>>;