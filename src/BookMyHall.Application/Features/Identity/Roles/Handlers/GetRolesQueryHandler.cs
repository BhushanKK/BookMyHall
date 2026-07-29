using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Constants;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetRolesQueryHandler(
    IRoleRepository roleRepository,
    IMapper mapper)
    : IRequestHandler<GetRolesQuery, ApiResponse<PaginatedResponse<Role>>>
{
    public async Task<ApiResponse<PaginatedResponse<Role>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await roleRepository.GetAllAsync(request.Request, cancellationToken);

        var response = new PaginatedResponse<Role>
        {
            Items = mapper.Map<IReadOnlyList<Role>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        return ApiResponse<PaginatedResponse<Role>>.SuccessResponse
        (
            response,
            string.Format(ApiMessages.RecordRetrieved, Entities.Role),
            HttpStatusCode.OK
        );
    }
}