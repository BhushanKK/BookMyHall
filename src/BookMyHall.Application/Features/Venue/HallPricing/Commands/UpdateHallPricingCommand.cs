using MediatR;

using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateHallPricingCommand
    : HallPricingDto, IRequest<ApiResponse<HallPricingDto>>;