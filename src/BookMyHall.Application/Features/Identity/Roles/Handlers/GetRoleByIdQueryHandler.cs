using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetRoleByIdQueryHandler(
    IRoleRepository roleRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<GetRoleByIdQuery, ApiResponse<Role>>
{
    public async Task<ApiResponse<Role>> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(
            request.RoleId,
            cancellationToken);

        if (role is null)
        {
            return ApiResponse<Role>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.Role),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<Role>.SuccessResponse(
            mapper.Map<Role>(role),
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Role),
            HttpStatusCode.OK);
    }
}