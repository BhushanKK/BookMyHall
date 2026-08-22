using System.Net;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class DeleteFoodTypeCommandHandler(
    IFoodTypeRepository foodTypeRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<DeleteFoodTypeCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteFoodTypeCommand request, CancellationToken cancellationToken)
    {
        var foodType = await foodTypeRepository.GetByIdAsync(request.FoodTypeId, cancellationToken);
        if (foodType is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFound(EntityKeys.FoodType),
                HttpStatusCode.NotFound);
        }

        foodType.IsActive = false;
        await foodTypeRepository.UpdateAsync(foodType, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Foodtype}:{request.FoodTypeId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.Foodtype}:", cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.FoodType), HttpStatusCode.OK);
    }
}