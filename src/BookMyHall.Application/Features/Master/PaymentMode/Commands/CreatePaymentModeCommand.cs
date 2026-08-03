using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class CreatePaymentModeCommand
    :PaymentModeDto, IRequest<ApiResponse<PaymentModeDto>>;