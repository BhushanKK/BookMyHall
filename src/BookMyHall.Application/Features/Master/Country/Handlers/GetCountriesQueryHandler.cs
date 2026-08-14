using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCountriesQueryHandler(
    ICountryRepository countryRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetCountriesQuery, ApiResponse<PaginatedResult<Country>>>
{
    public async Task<ApiResponse<PaginatedResult<Country>>> Handle(
        GetCountriesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await countryRepository.GetAllAsync(
            request.PaginationRequest,
            cancellationToken);

        var response = new PaginatedResult<Country>
        {
            Items = mapper.Map<IReadOnlyList<Country>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<Country>>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Country),
            HttpStatusCode.OK);
    }
}