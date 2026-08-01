using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record DeletePaymentModeCommand(Guid PaymentModeId)
    : IRequest<ApiResponse<bool>>;