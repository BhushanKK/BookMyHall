using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public record GetHallPricingByIdQuery(Guid HallPricingId)
    : IRequest<ApiResponse<HallPricingDto>>;