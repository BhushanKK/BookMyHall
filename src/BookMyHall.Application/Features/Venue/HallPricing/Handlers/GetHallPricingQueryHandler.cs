using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Venue;
public sealed class GetHallPricingQueryHandler(IHallPricingRepository hallPricingRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetHallPricingQuery,ApiResponse<PaginatedResult<HallPricingDto>>>
{
    public async Task<ApiResponse<PaginatedResult<HallPricingDto>>> Handle(
        GetHallPricingQuery request,
        CancellationToken cancellationToken)
    {
        var result = await hallPricingRepository.GetAllAsync(request.PaginationRequest, cancellationToken);

        var response = new PaginatedResult<HallPricingDto>
        {
            Items = mapper.Map<IReadOnlyList<HallPricingDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<HallPricingDto>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.HallPricing),
            HttpStatusCode.OK
        );
    }
}