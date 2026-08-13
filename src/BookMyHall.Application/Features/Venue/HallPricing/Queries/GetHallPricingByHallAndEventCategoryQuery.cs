using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetHallPricingByHallAndEventCategoryQuery(Guid HallId, Guid EventCategoryId)
    : IRequest<ApiResponse<HallPricingDto>>;