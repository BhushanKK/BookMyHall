using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetEventCategoryByIdQuery(Guid EventCategoryId)
    : IRequest<ApiResponse<EventCategoryDto>>;