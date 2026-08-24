using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetByIdMenuPermissionQueryHandler(
    IMenuPermissionRepository menuPermissionRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<
        GetByIdMenuPermissionQuery,
        ApiResponse<MenuPermissionDto>>
{
    public async Task<ApiResponse<MenuPermissionDto>> Handle(
        GetByIdMenuPermissionQuery request,
        CancellationToken cancellationToken)
    {
        var menuPermission =
            await menuPermissionRepository.GetByIdAsync(
                request.MenuPermissionId,
                cancellationToken);

        if (menuPermission is null)
        {
            return ApiResponse<MenuPermissionDto>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.MenuPermission),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<MenuPermissionDto>.SuccessResponse(
            mapper.Map<MenuPermissionDto>(menuPermission),
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.MenuPermission),
            HttpStatusCode.OK);
    }
}