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

public sealed class GetDistrictByIdQueryHandler(
    IDistrictRepository districtRepository,
    IMessageHelper messageHelper,
    IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetDistrictByIdQuery, ApiResponse<District>>
{
    public async Task<ApiResponse<District>> Handle(GetDistrictByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Districts}:{request.DistrictId}";
        var cachedDistrict = await cacheService.GetAsync<District>(cacheKey, cancellationToken);
        if (cachedDistrict is not null)
        {
            return ApiResponse<District>.SuccessResponse(cachedDistrict, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.District), HttpStatusCode.OK);
        }
        var district = await districtRepository.GetByIdAsync(request.DistrictId, cancellationToken);

        if (district is null)
        {
            return ApiResponse<District>.FailureResponse(
                messageHelper.NotFound(EntityKeys.District),
                HttpStatusCode.NotFound);
        }
        var response = mapper.Map<District>(district);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<District>.SuccessResponse(
            mapper.Map<District>(district),
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.District), HttpStatusCode.OK);
    }
}