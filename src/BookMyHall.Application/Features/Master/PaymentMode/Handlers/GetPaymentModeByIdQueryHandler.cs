using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class GetPaymentModeByIdQueryHandler(IPaymentModeRepository paymentModeRepository,
    IMessageHelper messageHelper,IMapper mapper,ICacheService cacheService)
    : IRequestHandler<GetPaymentModeByIdQuery, ApiResponse<PaymentMode>>
{
    public async Task<ApiResponse<PaymentMode>> Handle(GetPaymentModeByIdQuery request,CancellationToken cancellationToken)
    {
         var cacheKey = $"{CacheKeys.PaymentMode}:{request.PaymentModeId}";
        var cachedPaymentMode = await cacheService.GetAsync<PaymentMode>(cacheKey, cancellationToken);

        if (cachedPaymentMode is not null)
        {
            return ApiResponse<PaymentMode>.SuccessResponse
            (
                cachedPaymentMode,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.PaymentMode),
                HttpStatusCode.OK
            );
        }
        var paymentMode = await paymentModeRepository.GetByIdAsync(request.PaymentModeId,cancellationToken);

        if (paymentMode is null)
        {
            return ApiResponse<PaymentMode>.FailureResponse(
                messageHelper.NotFound(EntityKeys.PaymentMode),
                HttpStatusCode.NotFound);
        }
        var response = mapper.Map<PaymentMode>(paymentMode);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
       
        return ApiResponse<PaymentMode>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.OK);
    }
}