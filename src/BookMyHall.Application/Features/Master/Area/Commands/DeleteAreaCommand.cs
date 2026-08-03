using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record DeleteAreaCommand(Guid AreaId)
    : IRequest<ApiResponse<bool>>;