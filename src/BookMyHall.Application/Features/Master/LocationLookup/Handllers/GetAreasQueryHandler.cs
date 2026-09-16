using System.Net;
using FluentValidation;
using MediatR;
using BookMyHall.Domain.Dtos;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Master.LocationLookup.Queries;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Master.LocationLookup.Handlers;

public sealed class GetAreasQueryHandler(ILocationLookupRepository locationLookupRepository,
    IValidator<GetAreasQuery> validator, ICacheService cacheService)
    : IRequestHandler<GetAreasQuery, ApiResponse<IReadOnlyList<LocationLookupDto.AreaLookupDto>>>
{
    public async Task<ApiResponse<IReadOnlyList<LocationLookupDto.AreaLookupDto>>> Handle(GetAreasQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<IReadOnlyList<LocationLookupDto.AreaLookupDto>>
                .FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var cacheKey = $"{CacheKeys.AreasCached}:{request.CityId}";
        var cachedAreas = await cacheService.GetAsync<IReadOnlyList<LocationLookupDto.AreaLookupDto>>(cacheKey, cancellationToken);
        if (cachedAreas is not null)
        {
            return ApiResponse<IReadOnlyList<LocationLookupDto.AreaLookupDto>>
                .SuccessResponse(cachedAreas);
        }

        var areas = await locationLookupRepository.GetAreasAsync(request.CityId, cancellationToken);
        await cacheService.SetAsync(cacheKey, areas, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<IReadOnlyList<LocationLookupDto.AreaLookupDto>>
            .SuccessResponse(areas);
    }
}