using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetMenuQueryHandler(
    IMenuRepository menuRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<GetMenuQuery, ApiResponse<IReadOnlyList<Menu>>>
{
    public async Task<ApiResponse<IReadOnlyList<Menu>>> Handle(
        GetMenuQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Menus;

        var cachedMenus =
            await cacheService.GetAsync<IReadOnlyList<Menu>>(
                cacheKey,
                cancellationToken);

        if (cachedMenus is not null)
        {
            return ApiResponse<IReadOnlyList<Menu>>.SuccessResponse(
                cachedMenus,
                messageHelper.RetrievedEntity(
                    ResourceNames.Entities,
                    EntityKeys.Menu),
                HttpStatusCode.OK);
        }

        var menus = await menuRepository.GetAllAsync(
            cancellationToken);

        var response = mapper.Map<IReadOnlyList<Menu>>(menus);

        await cacheService.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(30),
            cancellationToken);

        return ApiResponse<IReadOnlyList<Menu>>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Menu),
            HttpStatusCode.OK);
    }
}