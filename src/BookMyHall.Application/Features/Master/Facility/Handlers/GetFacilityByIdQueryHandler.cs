using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class GetFacilityByIdQueryHandler(
    IFacilityRepository facilityRepository,
    IMessageHelper messageHelper,
    IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetFacilityByIdQuery, ApiResponse<Facility>>
{
    public async Task<ApiResponse<Facility>> Handle(GetFacilityByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Facilities}:{request.FacilityId}";
        var cachedFacility = await cacheService.GetAsync<Facility>(cacheKey, cancellationToken);

        if (cachedFacility is not null)
        {
            return ApiResponse<Facility>.SuccessResponse
            (
                cachedFacility,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Facility),
                HttpStatusCode.OK
            );
        }

        var facility = await facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
        if (facility is null)
        {
            return ApiResponse<Facility>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Facility),
                HttpStatusCode.NotFound);
        }
        var response = mapper.Map<Facility>(facility);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<Facility>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Facility), HttpStatusCode.OK);
    }
}