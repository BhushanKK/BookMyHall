using MediatR;

using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateHallPricingCommand 
    : HallPricingDto, IRequest<ApiResponse<HallPricingDto>>;