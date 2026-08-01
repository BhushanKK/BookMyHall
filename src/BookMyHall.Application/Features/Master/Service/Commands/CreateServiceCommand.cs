using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record CreateServiceCommand(string ServiceName,string ServiceIcon)
    : IRequest<ApiResponse<Guid>>;