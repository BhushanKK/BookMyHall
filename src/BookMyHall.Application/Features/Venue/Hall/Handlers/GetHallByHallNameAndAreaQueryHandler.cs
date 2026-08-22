using System.Net;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;
public sealed class GetHallByHallNameAndAreaQueryHandler(
    IHallRepository hallRepository,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetHallByHallNameAndAreaQuery, ApiResponse<Hall>>
{
    public async Task<ApiResponse<Hall>> Handle(
        GetHallByHallNameAndAreaQuery request,
        CancellationToken cancellationToken)
    {
         var cacheKey =
            $"{CacheKeys.Hall}:" +
            $"name:{request.HallName.Trim().ToLowerInvariant()}:" +
            $"area:{request.AreaId}";

            var cachedHall = await cacheService.GetAsync<Hall>(cacheKey,cancellationToken);

        if (cachedHall is not null)
        {
            return ApiResponse<Hall>.SuccessResponse(cachedHall,
                messageHelper.RetrievedEntity(ResourceNames.Entities,
                    EntityKeys.Hall),HttpStatusCode.OK);
        }
        var hall = await hallRepository.GetByHallNameAndAreaAsync(
            request.HallName,
            request.AreaId,
            cancellationToken);

        if (hall is null)
        {
            return ApiResponse<Hall>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.Hall),
                HttpStatusCode.NotFound
            );
        }
        await cacheService.SetAsync(cacheKey,hall,TimeSpan.FromMinutes(30),cancellationToken);
        return ApiResponse<Hall>.SuccessResponse
        ( hall,messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Hall),HttpStatusCode.OK);
    }
}