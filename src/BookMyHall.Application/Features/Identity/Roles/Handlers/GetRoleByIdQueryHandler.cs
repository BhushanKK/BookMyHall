using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Constants;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetRoleByIdQueryHandler(
    IRoleRepository roleRepository,
    IMapper mapper)
    : IRequestHandler<GetRoleByIdQuery, ApiResponse<Role>>
{
    public async Task<ApiResponse<Role>> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.RoleId,cancellationToken);

        if (role is null)
        {
            return ApiResponse<Role>.FailureResponse
            (
                string.Format(ApiMessages.RecordNotFound, Entities.Role),
                HttpStatusCode.NotFound
            );
        }

        return ApiResponse<Role>.SuccessResponse
        (
            mapper.Map<Role>(role),
            string.Format(ApiMessages.RecordRetrieved, Entities.Role),
            HttpStatusCode.OK
        );
    }
}