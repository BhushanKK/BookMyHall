using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetFacilitiesQueryHandler(
    IFacilityRepository facilityRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetFacilitiesQuery, ApiResponse<PaginatedResult<Facility>>>
{
    public async Task<ApiResponse<PaginatedResult<Facility>>> Handle(GetFacilitiesQuery request,CancellationToken cancellationToken)
    {
        var result = await facilityRepository.GetAllAsync(request.paginationRequest,cancellationToken);
        var response = new PaginatedResult<Facility>
        {
            Items = mapper.Map<IReadOnlyList<Facility>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<Facility>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Facility),HttpStatusCode.OK);
    }
}