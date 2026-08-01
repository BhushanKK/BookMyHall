using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetPaymentModesQueryHandler(
    IPaymentModeRepository paymentModeRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetPaymentModesQuery, ApiResponse<PaginatedResult<PaymentModeDto>>>
{
    public async Task<ApiResponse<PaginatedResult<PaymentModeDto>>> Handle(GetPaymentModesQuery request,CancellationToken cancellationToken)
    {
        var result = await paymentModeRepository.GetAllAsync(request.PaginationRequest,cancellationToken);
        var response = new PaginatedResult<PaymentModeDto>
        {
            Items = mapper.Map<List<PaymentModeDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<PaymentModeDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.OK);
    }
}