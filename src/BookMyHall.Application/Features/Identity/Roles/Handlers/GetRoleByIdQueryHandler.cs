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
        var cacheKey = $"{CacheKeys.Roles}:{request.RoleId}";

        var cachedRole = await cacheService.GetAsync<Role>(cacheKey, cancellationToken);

        if (cachedRole is not null)
        {
            return ApiResponse<Role>.SuccessResponse
            (
                cachedRole,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Role),
                HttpStatusCode.OK
            );
        }

        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return ApiResponse<Role>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.Role),
                HttpStatusCode.NotFound
            );
        }

        var response = mapper.Map<Role>(role);

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<Role>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Role),
            HttpStatusCode.OK
        );
    }
}