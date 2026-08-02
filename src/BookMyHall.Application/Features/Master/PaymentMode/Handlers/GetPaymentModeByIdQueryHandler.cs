using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetPaymentModeByIdQueryHandler(
    IPaymentModeRepository paymentModeRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetPaymentModeByIdQuery, ApiResponse<PaymentMode>>
{
    public async Task<ApiResponse<PaymentMode>> Handle(GetPaymentModeByIdQuery request,CancellationToken cancellationToken)
    {
        var paymentMode = await paymentModeRepository.GetByIdAsync(request.PaymentModeId,cancellationToken);

        if (paymentMode is null)
        {
            return ApiResponse<PaymentMode>.FailureResponse(
                messageHelper.NotFound(EntityKeys.PaymentMode),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<PaymentMode>.SuccessResponse(
            mapper.Map<PaymentMode>(paymentMode),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.OK);
    }
}