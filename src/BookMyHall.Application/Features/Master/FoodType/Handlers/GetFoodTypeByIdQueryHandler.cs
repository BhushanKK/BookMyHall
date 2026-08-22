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

public sealed class GetFoodTypeByIdQueryHandler(
    IFoodTypeRepository foodTypeRepository,
    IMessageHelper messageHelper,
    IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetFoodTypeByIdQuery, ApiResponse<FoodType>>
{
    public async Task<ApiResponse<FoodType>> Handle(GetFoodTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Foodtype}:{request.FoodTypeId}";
        var cachedFoodType = await cacheService.GetAsync<FoodType>(cacheKey, cancellationToken);

        if (cachedFoodType is not null)
        {
            return ApiResponse<FoodType>.SuccessResponse
            (
                cachedFoodType,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.FoodType),
                HttpStatusCode.OK
            );
        }

        var foodType = await foodTypeRepository.GetByIdAsync(request.FoodTypeId, cancellationToken);
        if (foodType is null)
        {
            return ApiResponse<FoodType>.FailureResponse(
                messageHelper.NotFound(EntityKeys.FoodType),
                HttpStatusCode.NotFound);
        }
        var response = mapper.Map<FoodType>(foodType);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<FoodType>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.FoodType), HttpStatusCode.OK);
    }
}