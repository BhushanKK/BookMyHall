using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetMenuQueryHandler(
    IMenuRepository menuRepository,
    IMapper mapper,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetMenuQuery,ApiResponse<PaginatedResponse<Menu>>>
{
    public async Task<ApiResponse<PaginatedResponse<Menu>>> Handle(
        GetMenuQuery request,CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;

        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<Menu>(
            CacheKeys.Menus,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<Menu>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<Menu>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Permission),
                HttpStatusCode.OK
            );
        }
        var pagedResult = await menuRepository.GetAllAsync(request.paginationRequest,cancellationToken);

        var response = new PaginatedResponse<Menu>
        {
            Items = mapper.Map<IReadOnlyList<Menu>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        
        return ApiResponse<PaginatedResponse<Menu>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Menu),
            HttpStatusCode.OK
        );
    }
    
}