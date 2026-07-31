using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record CreateCancellationPolicyCommand(string PolicyName,string Description,decimal RefundPercentage,int CancellationBeforeHours)
    : IRequest<ApiResponse<Guid>>;