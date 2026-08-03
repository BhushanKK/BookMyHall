using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed record GetServiceByIdQuery(Guid ServiceId)
    : IRequest<ApiResponse<Service>>;