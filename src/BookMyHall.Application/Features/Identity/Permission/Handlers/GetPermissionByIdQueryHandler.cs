using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetPermissionByIdQueryHandler(
    IPermissionRepository permissionRepository,
    IMapper mapper,IMessageHelper messageHelper)
    : IRequestHandler<GetPermissionByIdQuery, ApiResponse<PermissionDto>>
{
    public async Task<ApiResponse<PermissionDto>> Handle(GetPermissionByIdQuery request,CancellationToken cancellationToken)
    {
        var permission = await permissionRepository.GetByIdAsync(request.PermissionId,cancellationToken);

        if (permission is null)
        {
            return ApiResponse<PermissionDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.Permission),HttpStatusCode.NotFound);
        }

        return ApiResponse<PermissionDto>.SuccessResponse(mapper.Map<PermissionDto>(permission),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Permission),HttpStatusCode.OK);
    }
}