using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetPermissionByIdQueryHandler(
    IPermissionRepository permissionRepository,
    IMapper mapper,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetPermissionByIdQuery, ApiResponse<PermissionDto>>
{
    public async Task<ApiResponse<PermissionDto>> Handle(GetPermissionByIdQuery request,CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Permissions}:{request.PermissionId}";

        var cachedPermission = await cacheService.GetAsync<PermissionDto>(cacheKey, cancellationToken);

        if (cachedPermission is not null)
        {
            return ApiResponse<PermissionDto>.SuccessResponse
            (
                cachedPermission,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Permission),
                HttpStatusCode.OK
            );
        }

        var permission = await permissionRepository.GetByIdAsync(request.PermissionId,cancellationToken);

        if (permission is null)
        {
            return ApiResponse<PermissionDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.Permission),HttpStatusCode.NotFound);
        }
        var response = mapper.Map<PermissionDto>(permission);

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PermissionDto>.SuccessResponse(mapper.Map<PermissionDto>(permission),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Permission),HttpStatusCode.OK);
    }
}