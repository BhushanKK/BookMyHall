using System.Net;
using FluentValidation;
using MediatR;
using BookMyHall.Domain.Dtos;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Master.LocationLookup.Queries;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Master.LocationLookup.Handlers;

public sealed class GetStatesQueryHandler(ILocationLookupRepository locationLookupRepository,
    IValidator<GetStatesQuery> validator,ICacheService cacheService)
    : IRequestHandler<GetStatesQuery,ApiResponse<IReadOnlyList<LocationLookupDto.StateLookupDto>>>
{
    public async Task<ApiResponse<IReadOnlyList<LocationLookupDto.StateLookupDto>>>Handle( GetStatesQuery request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<IReadOnlyList<LocationLookupDto.StateLookupDto>>
                .FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var cacheKey =$"{CacheKeys.StatesCached}:{request.CountryId}";
        var cachedStates =await cacheService.GetAsync<IReadOnlyList<LocationLookupDto.StateLookupDto>>(cacheKey,cancellationToken);
        if (cachedStates is not null)
        {
            return ApiResponse<IReadOnlyList<LocationLookupDto.StateLookupDto>>
                .SuccessResponse(cachedStates);
        }

        var states =await locationLookupRepository.GetStatesAsync( request.CountryId,cancellationToken);
        await cacheService.SetAsync(cacheKey,states,TimeSpan.FromMinutes(30),cancellationToken);
        return ApiResponse<IReadOnlyList<LocationLookupDto.StateLookupDto>>
            .SuccessResponse(states);
    }
}