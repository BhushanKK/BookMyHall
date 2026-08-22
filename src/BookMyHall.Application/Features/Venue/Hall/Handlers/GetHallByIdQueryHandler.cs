using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Venue;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallByIdQueryHandler(IHallRepository hallRepository,
    IMapper mapper,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetHallByIdQuery, ApiResponse<Hall>>
{
    public async Task<ApiResponse<Hall>> Handle(GetHallByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Hall}:{request.HallId}";
        var cachedHall = await cacheService.GetAsync<Hall>(cacheKey, cancellationToken);

        if (cachedHall is not null)
        {
            return ApiResponse<Hall>.SuccessResponse
            (
                cachedHall,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Hall),
                HttpStatusCode.OK
            );
        }
        var hall = await hallRepository.GetByIdAsync(request.HallId, cancellationToken);

        if (hall is null)
        {
            return ApiResponse<Hall>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.Hall),
                HttpStatusCode.NotFound
            );
        }
        var response = mapper.Map<Hall>(hall);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<Hall>.SuccessResponse
        (response ,messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Hall),
            HttpStatusCode.OK
        );
    }
}