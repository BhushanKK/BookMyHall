using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetPaymentModesQueryHandler(
    IPaymentModeRepository paymentModeRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetPaymentModesQuery, ApiResponse<PaginatedResult<PaymentMode>>>
{
    public async Task<ApiResponse<PaginatedResult<PaymentMode>>> Handle(GetPaymentModesQuery request,CancellationToken cancellationToken)
    {
        var result = await paymentModeRepository.GetAllAsync(request.paginationRequest,cancellationToken);
        var response = new PaginatedResult<PaymentMode>
        {
            Items = mapper.Map<IReadOnlyList<PaymentMode>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<PaymentMode>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.OK);
    }
}