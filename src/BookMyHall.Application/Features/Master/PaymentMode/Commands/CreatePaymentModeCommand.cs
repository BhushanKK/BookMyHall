using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record CreatePaymentModeCommand(string PaymentModeName)
    : IRequest<ApiResponse<Guid>>;