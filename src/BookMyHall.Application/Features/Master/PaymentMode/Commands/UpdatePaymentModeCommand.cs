using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Master;

public sealed class UpdatePaymentModeCommand()
    :PaymentModeDto, IRequest<ApiResponse<PaymentModeDto>>;
