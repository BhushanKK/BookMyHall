using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetPaymentModeByIdQueryHandler(
    IPaymentModeRepository paymentModeRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetPaymentModeByIdQuery, ApiResponse<PaymentModeDto>>
{
    public async Task<ApiResponse<PaymentModeDto>> Handle(GetPaymentModeByIdQuery request,CancellationToken cancellationToken)
    {
        var paymentMode = await paymentModeRepository.GetByIdAsync(request.PaymentModeId,cancellationToken);

        if (paymentMode is null)
        {
            return ApiResponse<PaymentModeDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.PaymentMode),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<PaymentModeDto>.SuccessResponse(
            mapper.Map<PaymentModeDto>(paymentMode),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.OK);
    }
}