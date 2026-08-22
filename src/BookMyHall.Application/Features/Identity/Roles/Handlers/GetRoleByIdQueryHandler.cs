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

public sealed class GetRoleByIdQueryHandler(
    IRoleRepository roleRepository,
    IMapper mapper,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<GetRoleByIdQuery, ApiResponse<Role>>
{
    public async Task<ApiResponse<Role>> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Country}:{request.RoleId}";
        var cachedRole = await cacheService.GetAsync<Role>(cacheKey, cancellationToken);
        
        if (cachedRole is not null)
        {
            return ApiResponse<Role>.SuccessResponse
            (
                cachedRole,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Country),
                HttpStatusCode.OK
            );
        }

        var role = await roleRepository.GetByIdAsync(request.RoleId,cancellationToken);

        await cacheService.RemoveAsync($"{CacheKeys.Roles}:{request.RoleId}", cancellationToken);

        await cacheService.RemoveByPrefixAsync($"{CacheKeys.Roles}:page:", cancellationToken);

        if (role is null)
        {
            return ApiResponse<Role>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.Role),
                HttpStatusCode.NotFound
            );
        }

        return ApiResponse<Role>.SuccessResponse
        (
            mapper.Map<Role>(role),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Role),
            HttpStatusCode.OK
        );
    }
}