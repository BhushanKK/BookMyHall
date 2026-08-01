using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetDistrictsQueryHandler(
    IDistrictRepository districtRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetDistrictsQuery, ApiResponse<PaginatedResult<DistrictDto>>>
{
    public async Task<ApiResponse<PaginatedResult<DistrictDto>>> Handle(GetDistrictsQuery request,CancellationToken cancellationToken)
    {
        var result = await districtRepository.GetAllAsync(request.Request,cancellationToken);
        var response = new PaginatedResult<DistrictDto>
        {
            Items = mapper.Map<List<DistrictDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<DistrictDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.District),HttpStatusCode.OK);
    }
}