using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetCancellationPolicyByIdQuery(Guid CancellationPolicyId)
    : IRequest<ApiResponse<CancellationPolicyDto>>;