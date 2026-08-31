using MediatR;

using System.Net;

using AutoMapper;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class GetAmenityByIdQueryHandler(
    IAmenityRepository amenityRepository,
    IMapper mapper,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetAmenityByIdQuery, ApiResponse<Amenity>>
{
    public async Task<ApiResponse<Amenity>> Handle(GetAmenityByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Amenity}:{request.AmenityId}";
        var cachedAmenity = await cacheService.GetAsync<Amenity>(cacheKey, cancellationToken);
        if (cachedAmenity is not null)
        {
            return ApiResponse<Amenity>.SuccessResponse(cachedAmenity, messageHelper.RetrievedEntity
            (ResourceNames.Entities, EntityKeys.Amenity), HttpStatusCode.OK);
        }
        var Amenity = await amenityRepository.GetByIdAsync(request.AmenityId, cancellationToken);
        if (Amenity is null)
        {
            return ApiResponse<Amenity>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.Amenity),
                HttpStatusCode.NotFound
            );
        }
        var response = mapper.Map<Amenity>(Amenity);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<Amenity>.SuccessResponse
        (
            mapper.Map<Amenity>(Amenity),
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Amenity),
            HttpStatusCode.OK
        );
    }
}