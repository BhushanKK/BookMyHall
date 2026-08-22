using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallCoverImageQueryHandler(
    IHallImageRepository hallImageRepository,
    IMapper mapper,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetHallCoverImageQuery, ApiResponse<HallImageDto>>
{
    public async Task<ApiResponse<HallImageDto>> Handle(
        GetHallCoverImageQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.HallImage}:{request.HallId}";
        var cachedHallImage = await cacheService.GetAsync<HallImageDto>(cacheKey, cancellationToken);

        if (cachedHallImage is not null)
        {
            return ApiResponse<HallImageDto>.SuccessResponse
            (
                cachedHallImage,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallImage),
                HttpStatusCode.OK
            );
        }
        var coverImage = await hallImageRepository.GetCoverImageAsync(request.HallId, cancellationToken);

        if (coverImage is null)
        {
            return ApiResponse<HallImageDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.HallImage),
                HttpStatusCode.NotFound
            );
        }
        var response = mapper.Map<HallImageDto>(coverImage);
        await cacheService.SetAsync(cacheKey,response,TimeSpan.FromMinutes(30),cancellationToken);

        return ApiResponse<HallImageDto>.SuccessResponse
        (
            mapper.Map<HallImageDto>(coverImage),
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallImage),
            HttpStatusCode.OK
        );
    }
}