using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetByIdMenuQueryHandler(
    IMenuRepository menuRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<GetByIdMenuQuery,ApiResponse<Menu>>
{
    public async Task<ApiResponse<Menu>> Handle(   
        GetByIdMenuQuery request,
        CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetByIdAsync(request.MenuId,cancellationToken);

        if (menu is null)
        {
            return ApiResponse<Menu>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.Menu),
                HttpStatusCode.NotFound
            );
        }

        return ApiResponse<Menu>.SuccessResponse
        (
            mapper.Map<Menu>(menu),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Menu),
            HttpStatusCode.OK
        );
    }
}
