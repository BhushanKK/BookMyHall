using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed record GetPaymentModeByIdQuery(Guid PaymentModeId)
    : IRequest<ApiResponse<PaymentMode>>;