using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetAllMenuPermissionQueryHandler(
    IMenuPermissionRepository menuPermissionRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<
        GetAllMenuPermissionQuery,
        ApiResponse<PaginatedResponse<MenuPermissionDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<MenuPermissionDto>>> Handle(
        GetAllMenuPermissionQuery request,
        CancellationToken cancellationToken)
    {
        var result = await menuPermissionRepository.GetAllAsync(
            request.Request,
            cancellationToken);

        var items = mapper.Map<List<MenuPermissionDto>>(
            result.Items);

        var response = new PaginatedResponse<MenuPermissionDto>
        {
            Items = items,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResponse<MenuPermissionDto>>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.MenuPermission),
            HttpStatusCode.OK);
    }
}