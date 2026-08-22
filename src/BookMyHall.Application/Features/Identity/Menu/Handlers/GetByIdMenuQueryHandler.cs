using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetByIdMenuQueryHandler(
    IMenuRepository menuRepository,
    IMapper mapper,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetByIdMenuQuery,ApiResponse<Menu>>
{
    public async Task<ApiResponse<Menu>> Handle(   
        GetByIdMenuQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Menus}:{request.MenuId}";

        var cachedMenu = await cacheService.GetAsync<Menu>(cacheKey, cancellationToken);

        if (cachedMenu is not null)
        {
            return ApiResponse<Menu>.SuccessResponse
            (
                cachedMenu,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Menu),
                HttpStatusCode.OK
            );
        }

        var menu = await menuRepository.GetByIdAsync(request.MenuId,cancellationToken);

        if (menu is null)
        {
            return ApiResponse<Menu>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.Menu),
                HttpStatusCode.NotFound
            );
        }
        var response = mapper.Map<PermissionDto>(menu);

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        
        return ApiResponse<Menu>.SuccessResponse
        (
            mapper.Map<Menu>(menu),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Menu),
            HttpStatusCode.OK
        );
    }
}
