using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallBlockByIdQueryHandler(IHallBlockRepository hallBlockRepository,
    IMapper mapper, IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetHallBlockByIdQuery, ApiResponse<HallBlock>>
{
    public async Task<ApiResponse<HallBlock>> Handle(GetHallBlockByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.HallBlock}:{request.HallBlockId}";
        var cachedHallBlock = await cacheService.GetAsync<HallBlock>(cacheKey, cancellationToken);

        if (cachedHallBlock is not null)
        {
            return ApiResponse<HallBlock>.SuccessResponse
            (
                cachedHallBlock,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallBlock),
                HttpStatusCode.OK
            );
        }
        var hallBlock = await hallBlockRepository.GetByIdAsync(request.HallBlockId, cancellationToken);

        if (hallBlock is null)
        {
            return ApiResponse<HallBlock>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities, EntityKeys.HallBlock), HttpStatusCode.NotFound);
        }
        var response = mapper.Map<HallBlock>(hallBlock);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<HallBlock>.SuccessResponse(mapper.Map<HallBlock>(hallBlock),
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallBlock), HttpStatusCode.OK);
    }
}