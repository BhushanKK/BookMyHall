using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;
public sealed class DeleteEventCategoryCommandHandler(IEventCategoryRepository eventCategoryRepository,
    IUnitOfWork unitOfWork,IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<DeleteEventCategoryCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteEventCategoryCommand request, CancellationToken cancellationToken)
    {
        var eventCategory = await eventCategoryRepository.GetByIdAsync(request.EventCategoryId, cancellationToken);
        if (eventCategory is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFound(EntityKeys.EventCategory),
                HttpStatusCode.NotFound);
        }

        eventCategory.IsDeleted = true;
        eventCategory.IsActive=false;
        await eventCategoryRepository.UpdateAsync(eventCategory, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        var cacheKey = $"{CacheKeys.EventCategories}:{request.EventCategoryId}";
        await cacheService.RemoveAsync(cacheKey, cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.EventCategoriesPaged}:", cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.EventCategory), HttpStatusCode.OK);
    }
}