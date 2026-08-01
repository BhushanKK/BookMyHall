using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record CreateEventCategoryCommand(string EventCategoryName)
    : IRequest<ApiResponse<Guid>>;