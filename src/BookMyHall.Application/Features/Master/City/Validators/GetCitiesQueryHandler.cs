using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCitiesQueryHandler(
    ICityRepository cityRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetCitiesQuery, ApiResponse<PaginatedResult<CityDto>>>
{
    public async Task<ApiResponse<PaginatedResult<CityDto>>> Handle(GetCitiesQuery request,CancellationToken cancellationToken)
    {
        var result = await cityRepository.GetAllAsync(request.Request,cancellationToken);

        var response = new PaginatedResult<CityDto>
        {
            Items = mapper.Map<IReadOnlyList<CityDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<CityDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.City),HttpStatusCode.OK);
    }
}