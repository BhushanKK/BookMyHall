using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record DeleteCancellationPolicyCommand(Guid CancellationPolicyId)
    : IRequest<ApiResponse<bool>>;