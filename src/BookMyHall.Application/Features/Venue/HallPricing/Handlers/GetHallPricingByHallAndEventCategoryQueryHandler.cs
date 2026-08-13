using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallPricingByHallAndEventCategoryQueryHandler(
    IHallPricingRepository hallPricingRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<GetHallPricingByHallAndEventCategoryQuery, ApiResponse<HallPricingDto>>
{
    public async Task<ApiResponse<HallPricingDto>> Handle(
        GetHallPricingByHallAndEventCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var hallPricing = await hallPricingRepository.GetByHallIdAndEventCategoryIdAsync
        (
            request.HallId,
            request.EventCategoryId,
            cancellationToken
        );

        if (hallPricing is null)
        {
            return ApiResponse<HallPricingDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.HallPricing),
                HttpStatusCode.NotFound
            );
        }

        return ApiResponse<HallPricingDto>.SuccessResponse
        (
            mapper.Map<HallPricingDto>(hallPricing),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.HallPricing),
            HttpStatusCode.OK
        );
    }
}