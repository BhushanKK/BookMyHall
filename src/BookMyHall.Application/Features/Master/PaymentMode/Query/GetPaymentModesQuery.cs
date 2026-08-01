using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetPaymentModesQuery(PaginationRequest PaginationRequest)
    : IRequest<ApiResponse<PaginatedResult<PaymentModeDto>>>;