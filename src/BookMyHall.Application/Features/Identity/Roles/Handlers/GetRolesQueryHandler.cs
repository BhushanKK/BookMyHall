using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetRolesQueryHandler(
    IRoleRepository roleRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<GetRolesQuery, ApiResponse<PaginatedResponse<Role>>>
{
    public async Task<ApiResponse<PaginatedResponse<Role>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await roleRepository.GetAllAsync(
            request.Request,
            cancellationToken);

        var response = new PaginatedResponse<Role>
        {
            Items = mapper.Map<IReadOnlyList<Role>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        return ApiResponse<PaginatedResponse<Role>>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Role),
            HttpStatusCode.OK);
    }
}