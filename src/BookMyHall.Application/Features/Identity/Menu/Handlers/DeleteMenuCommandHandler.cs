using MediatR;
using System.Net;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity;

public sealed class DeleteMenuCommandHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<DeleteMenuCommand,ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteMenuCommand request,
        CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetByIdAsync(request.MenuId,cancellationToken);

        if(menu is null)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,
                EntityKeys.Menu),
                HttpStatusCode.NotFound
            );
        }

        await menuRepository.UpdateAsync(menu,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync( $"{CacheKeys.Menus}:{request.MenuId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.MenuPaged}:",cancellationToken);
        return ApiResponse<bool>.SuccessResponse
        (
            true,messageHelper.DeletedEntity(ResourceNames.Entities,EntityKeys.Menu),
            HttpStatusCode.OK
        );
    }
}