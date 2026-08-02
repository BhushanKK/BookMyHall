using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetDistrictsQueryHandler(
    IDistrictRepository districtRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetDistrictsQuery, ApiResponse<PaginatedResult<District>>>
{
    public async Task<ApiResponse<PaginatedResult<District>>> Handle(GetDistrictsQuery request,CancellationToken cancellationToken)
    {
        var result = await districtRepository.GetAllAsync(request.pagingRequest,cancellationToken);
        var response = new PaginatedResult<District>
        {
            Items = mapper.Map<IReadOnlyList<District>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<District>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.District),HttpStatusCode.OK);
    }
}