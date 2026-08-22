using System.Net;

using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class GetHallCategoryByIdQueryHandler(
    IHallCategoryRepository hallCategoryRepository,
    IMapper mapper,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<GetHallCategoryByIdQuery, ApiResponse<HallCategory>>
{
    public async Task<ApiResponse<HallCategory>> Handle(GetHallCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.HallCategory}:{request.HallCategoryId}";
        var cachedHallCategory = await cacheService.GetAsync<HallCategory>(cacheKey, cancellationToken);

        if (cachedHallCategory is not null)
        {
            return ApiResponse<HallCategory>.SuccessResponse
            (
                cachedHallCategory,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallCategory),
                HttpStatusCode.OK
            );
        }
        var category = await hallCategoryRepository.GetByIdAsync(request.HallCategoryId, cancellationToken);

        if (category is null)
        {
            return ApiResponse<HallCategory>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.HallCategory), HttpStatusCode.NotFound);
        }
        var response = mapper.Map<HallCategory>(category);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<HallCategory>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.HallCategory), HttpStatusCode.OK);
    }
}