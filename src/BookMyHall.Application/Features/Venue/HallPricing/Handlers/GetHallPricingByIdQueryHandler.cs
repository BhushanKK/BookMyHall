using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallPricingByIdQueryHandler(
    IHallPricingRepository hallPricingRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<
        GetHallPricingByIdQuery,
        ApiResponse<HallPricingDto>>
{
    public async Task<ApiResponse<HallPricingDto>> Handle(
        GetHallPricingByIdQuery request,
        CancellationToken cancellationToken)
    {
        var hallPricing =
            await hallPricingRepository.GetByIdAsync(
                request.HallPricingId,
                cancellationToken);

        if (hallPricing is null)
        {
            return ApiResponse<HallPricingDto>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallPricing),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<HallPricingDto>.SuccessResponse(
            mapper.Map<HallPricingDto>(hallPricing),
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.HallPricing),
            HttpStatusCode.OK);
    }
}