using System.Net;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class DeleteAreaCommandHandler(
    IAreaRepository areaRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<DeleteAreaCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteAreaCommand request, CancellationToken cancellationToken)
    {
        var area = await areaRepository.GetByIdAsync(request.AreaId, cancellationToken);

        if (area is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFound(EntityKeys.Area), HttpStatusCode.NotFound);
        }

        area.IsDeleted = true;
        await areaRepository.UpdateAsync(area, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Areas}:{request.AreaId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.AreasPaged}:", cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.Area), HttpStatusCode.OK);
    }
}