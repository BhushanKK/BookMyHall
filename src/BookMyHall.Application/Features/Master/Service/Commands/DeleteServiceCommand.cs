using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record DeleteServiceCommand(Guid ServiceId)
    : IRequest<ApiResponse<bool>>;