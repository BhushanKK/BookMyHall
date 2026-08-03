using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCitiesQueryHandler(
    ICityRepository cityRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetCitiesQuery, ApiResponse<PaginatedResult<City>>>
{
    public async Task<ApiResponse<PaginatedResult<City>>> Handle(GetCitiesQuery request,CancellationToken cancellationToken)
    {
        var result = await cityRepository.GetAllAsync(request.paginationRequest,cancellationToken);
        var response = new PaginatedResult<City>
        {
            Items = mapper.Map<IReadOnlyList<City>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<City>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.City),HttpStatusCode.OK);
    }
}