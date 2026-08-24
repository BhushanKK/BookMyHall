using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

public sealed class GetMenusByRoleIdQueryHandler(
    IMessageHelper messageHelper,
    IMenuRepository menuRepository)
    : IRequestHandler<GetMenusByRoleIdQuery, ApiResponse<IReadOnlyList<Menu>>>
{
    public async Task<ApiResponse<IReadOnlyList<Menu>>> Handle(
        GetMenusByRoleIdQuery request,
        CancellationToken cancellationToken)
    {
        var menus = await menuRepository.GetByRoleIdAsync(request.RoleId, cancellationToken);
      
        return ApiResponse<IReadOnlyList<Menu>>.SuccessResponse
        (
            menus,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Menu),
            HttpStatusCode.OK
        );
    }
}