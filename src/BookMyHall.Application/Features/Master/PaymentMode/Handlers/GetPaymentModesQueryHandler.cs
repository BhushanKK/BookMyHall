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
    : IRequestHandler<GetPaymentModesQuery, ApiResponse<PaginatedResponse<PaymentMode>>>
{
    public async Task<ApiResponse<PaginatedResponse<PaymentMode>>> Handle(GetPaymentModesQuery request,CancellationToken cancellationToken)
    {
         var pagination = request.paginationRequest;
         var cacheKey = CacheKeyBuilder.BuildPaginatedKey<PaymentMode>(
            CacheKeys.PaymentModesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<PaymentMode>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<PaymentMode>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.PaymentMode),
                HttpStatusCode.OK
            );
        }
        var result = await paymentModeRepository.GetAllAsync(request.paginationRequest,cancellationToken);
        var response = new PaginatedResponse<PaymentMode>
        {
            Items = mapper.Map<IReadOnlyList<PaymentMode>>(result.Items),
            TotalRecords = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<PaymentMode>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.OK);
    }
}