using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetMenuQueryHandler(
    IMenuRepository menuRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<GetMenuQuery,ApiResponse<PaginatedResponse<Menu>>>
{
    public async Task<ApiResponse<PaginatedResponse<Menu>>> Handle(
        GetMenuQuery request,CancellationToken cancellationToken)
    {
        var pagedResult = await menuRepository.GetAllAsync(request.Request,cancellationToken);

        var response = new PaginatedResponse<Menu>
        {
            Items = mapper.Map<IReadOnlyList<Menu>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        return ApiResponse<PaginatedResponse<Menu>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Menu),
            HttpStatusCode.OK
        );
    }
    
}