using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCountryByIdQueryHandler(
    ICountryRepository countryRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetCountryByIdQuery, ApiResponse<Country>>
{
    public async Task<ApiResponse<Country>> Handle(
        GetCountryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var country = await countryRepository.GetByIdAsync(
            request.CountryId,
            cancellationToken);

        if (country is null)
        {
            return ApiResponse<Country>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Country),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<Country>.SuccessResponse(
            mapper.Map<Country>(country),
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Country),
            HttpStatusCode.OK);
    }
}