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

public sealed class GetFoodTypesQueryHandler(
    IFoodTypeRepository foodTypeRepository,
    IMessageHelper messageHelper,
    IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetFoodTypesQuery, ApiResponse<PaginatedResult<FoodType>>>
{
    public async Task<ApiResponse<PaginatedResult<FoodType>>> Handle(GetFoodTypesQuery request, CancellationToken cancellationToken)
    {

        var pagination = request.paginationRequest;

          var cacheKey = CacheKeyBuilder.BuildPaginatedKey<FoodType>(
            CacheKeys.Foodtype,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResult<FoodType>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResult<FoodType>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.FoodType),
                HttpStatusCode.OK
            );
        }

        var result = await foodTypeRepository.GetAllAsync(request.paginationRequest, cancellationToken);
        var response = new PaginatedResult<FoodType>
        {
            Items = mapper.Map<IReadOnlyList<FoodType>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<PaginatedResult<FoodType>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.FoodType), HttpStatusCode.OK);
    }
}