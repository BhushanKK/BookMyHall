using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;
public sealed class GetHallImageByIdQueryHandler(
    IHallImageRepository hallImageRepository,IMapper mapper,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetHallImageByIdQuery, ApiResponse<HallImageDto>>
{
    public async Task<ApiResponse<HallImageDto>> Handle(
        GetHallImageByIdQuery request,
        CancellationToken cancellationToken)
    {
         var cacheKey = $"{CacheKeys.HallImage}:{request.HallImageId}";
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
        var hallImage = await hallImageRepository.GetByIdAsync(request.HallImageId, cancellationToken);

        if (hallImage is null)
        {
            return ApiResponse<HallImageDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.HallImage),
                HttpStatusCode.NotFound
            );
        }

        var response = mapper.Map<HallImage>(hallImage);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<HallImageDto>.SuccessResponse
        (
            mapper.Map<HallImageDto>(hallImage),
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallImage),
            HttpStatusCode.OK
        );
    }
}