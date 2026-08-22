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

public sealed class GetPaymentModesQueryHandler(IPaymentModeRepository paymentModeRepository,
    IMessageHelper messageHelper,IMapper mapper,ICacheService cacheService)
    : IRequestHandler<GetPaymentModesQuery, ApiResponse<PaginatedResult<PaymentMode>>>
{
    public async Task<ApiResponse<PaginatedResult<PaymentMode>>> Handle(GetPaymentModesQuery request,CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;

        var cacheKey =
            $"{CacheKeys.PaymentMode}:" +
            $"page:{pagination.PageNumber}:" +
            $"size:{pagination.PageSize}";

        var cachedResponse = await cacheService.GetAsync<PaginatedResult<PaymentMode>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResult<PaymentMode>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.PaymentMode),
                HttpStatusCode.OK
            );
        }
        var result = await paymentModeRepository.GetAllAsync(request.paginationRequest,cancellationToken);
        var response = new PaginatedResult<PaymentMode>
        {
            Items = mapper.Map<IReadOnlyList<PaymentMode>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResult<PaymentMode>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.OK);
    }
}