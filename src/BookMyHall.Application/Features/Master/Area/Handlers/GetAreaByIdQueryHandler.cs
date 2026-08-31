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

public sealed class GetAreaByIdQueryHandler(
    IAreaRepository areaRepository,
    IMessageHelper messageHelper,
    IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetAreaByIdQuery, ApiResponse<Area>>
{
    public async Task<ApiResponse<Area>> Handle(GetAreaByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Areas}:{request.AreaId}";
        var cachedArea = await cacheService.GetAsync<Area>(cacheKey, cancellationToken);
        if (cachedArea is not null)
        {
            return ApiResponse<Area>.SuccessResponse(cachedArea, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.Area), HttpStatusCode.OK);
        }
        var area = await areaRepository.GetByIdAsync(request.AreaId, cancellationToken);
        if (area is null)
        {
            return ApiResponse<Area>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Area),
                HttpStatusCode.NotFound);
        }
        var response = mapper.Map<Area>(area);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<Area>.SuccessResponse(
            mapper.Map<Area>(area),
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Area), HttpStatusCode.OK);
    }
}