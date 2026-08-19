using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetPermissionQueryHandler(
    IPermissionRepository permissionRepository,
    IMapper mapper,IMessageHelper messageHelper)
    : IRequestHandler<GetPermissionQuery,ApiResponse<PaginatedResponse<Permission>>>
{
    public async Task<ApiResponse<PaginatedResponse<Permission>>> Handle(
        GetPermissionQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await permissionRepository.GetAllAsync(request.paginationRequest,cancellationToken);

        var response = new PaginatedResponse<Permission>
        {
            Items = mapper.Map<IReadOnlyList<Permission>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        return ApiResponse<PaginatedResponse<Permission>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities, EntityKeys.Permission), HttpStatusCode.OK);
    }
}