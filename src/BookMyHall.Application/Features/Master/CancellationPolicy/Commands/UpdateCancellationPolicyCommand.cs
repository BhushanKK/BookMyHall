using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateCancellationPolicyCommand()
    :CancellationPolicyDto, IRequest<ApiResponse<CancellationPolicyDto>>;
