using System.Net;
using AutoMapper;
using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallBlocksQueryHandler(IHallBlockRepository hallBlockRepository,
    IMapper mapper,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetHallBlocksQuery,ApiResponse<PaginatedResponse<HallBlockDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<HallBlockDto>>> Handle(GetHallBlocksQuery request,CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;
        var cacheKey =
            $"{CacheKeys.HallBlock}:" +
            $"page:{pagination.PageNumber}:" +
            $"size:{pagination.PageSize}";

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<HallBlockDto>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<HallBlockDto>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallBlock),
                HttpStatusCode.OK
            );
        }
        var pagedResult = await hallBlockRepository.GetAllAsync(request.paginationRequest,cancellationToken);
        var response = new PaginatedResponse<HallBlockDto>
        {
            Items = mapper.Map<IReadOnlyList<HallBlockDto>>(
                pagedResult.Items),

            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<PaginatedResponse<HallBlockDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.HallBlock),HttpStatusCode.OK);
    }
}