using MediatR;

using System.Net;

using AutoMapper;

using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class GetAmenityQueryHandler(
    IAmenityRepository amenityRepository,
    IMapper mapper,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetAmenitiesQuery, ApiResponse<PaginatedResponse<Amenity>>>
{
    public async Task<ApiResponse<PaginatedResponse<Amenity>>> Handle(GetAmenitiesQuery request, CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;

        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<Amenity>(
            CacheKeys.AmenitiesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<Amenity>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<Amenity>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Amenity),
                HttpStatusCode.OK
            );
        }
        var pagedResult = await amenityRepository.GetAllAsync(request.paginationRequest, cancellationToken);

        var response = new PaginatedResponse<Amenity>
        {
            Items = mapper.Map<IReadOnlyList<Amenity>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<Amenity>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Amenity),
            HttpStatusCode.OK
        );
    }
}