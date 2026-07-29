using AutoMapper;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetRolesQueryHandler(
    IRoleRepository roleRepository,
    IMapper mapper)
    : IRequestHandler<GetRolesQuery,ApiResponse<PaginatedResponse<RoleDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<RoleDto>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await roleRepository.GetAllAsync(request.Request,cancellationToken);

        var response = new PaginatedResponse<RoleDto>
        {
            Items = mapper.Map<IReadOnlyList<RoleDto>>(result.Items),
            PageNumber = request.Request.PageNumber,
            PageSize = request.Request.PageSize,
            TotalRecords = result.TotalCount
        };

        return ApiResponse<PaginatedResponse<RoleDto>>.Success
        (
            response,
            string.Format(ApiMessages.RecordRetrieved,Entities.Role)
        );
    }
}