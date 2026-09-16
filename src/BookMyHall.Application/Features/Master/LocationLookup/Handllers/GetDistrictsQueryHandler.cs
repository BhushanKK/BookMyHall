using System.Net;
using FluentValidation;
using MediatR;
using BookMyHall.Domain.Dtos;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Master.LocationLookup.Queries;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Master.LocationLookup.Handlers;

public sealed class GetDistrictsQueryHandler(ILocationLookupRepository locationLookupRepository,
    IValidator<GetDistrictsQuery> validator,ICacheService cacheService)
    : IRequestHandler< GetDistrictsQuery,ApiResponse<IReadOnlyList<LocationLookupDto.DistrictLookupDto>>>
{
    public async Task<ApiResponse<IReadOnlyList<LocationLookupDto.DistrictLookupDto>>> Handle(GetDistrictsQuery request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<IReadOnlyList<LocationLookupDto.DistrictLookupDto>>
                .FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var cacheKey =$"{CacheKeys.DistrictsCached}:{request.StateId}";
        var cachedDistricts =await cacheService.GetAsync<IReadOnlyList<LocationLookupDto.DistrictLookupDto>>(cacheKey,cancellationToken);
        if (cachedDistricts is not null)
        {
            return ApiResponse<IReadOnlyList<LocationLookupDto.DistrictLookupDto>>
                .SuccessResponse(cachedDistricts);
        }

        var districts =await locationLookupRepository.GetDistrictsAsync(request.StateId,cancellationToken);

       await cacheService.SetAsync(cacheKey,districts,TimeSpan.FromMinutes(30),cancellationToken);

        return ApiResponse<IReadOnlyList<LocationLookupDto.DistrictLookupDto>>
            .SuccessResponse(districts);
    }
}