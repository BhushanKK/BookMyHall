using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed record GetPaymentModesQuery(PaginationRequest paginationRequest)
    : IRequest<ApiResponse<PaginatedResult<PaymentMode>>>;