using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetServiceByIdQuery(Guid ServiceId)
    : IRequest<ApiResponse<ServiceDto>>;