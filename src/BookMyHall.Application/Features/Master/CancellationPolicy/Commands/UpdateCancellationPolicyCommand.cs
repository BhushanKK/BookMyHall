using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateCancellationPolicyCommand
    : IRequest<ApiResponse<CancellationPolicyDto>>
{
    public Guid CancellationPolicyId { get; set; }

    public string PolicyName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal RefundPercentage { get; set; }

    public int CancellationBeforeHours { get; set; }

    public bool IsActive { get; set; }
}