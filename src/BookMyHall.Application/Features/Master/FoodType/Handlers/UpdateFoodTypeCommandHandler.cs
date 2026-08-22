using System.Net;

using AutoMapper;

using FluentValidation;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Persistence.Exceptions;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateFoodTypeCommandHandler(
    IFoodTypeRepository foodTypeRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateFoodTypeCommand> validator,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<UpdateFoodTypeCommand, ApiResponse<FoodTypeDto>>
{
    public async Task<ApiResponse<FoodTypeDto>> Handle(UpdateFoodTypeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<FoodTypeDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var foodType = await foodTypeRepository.GetByIdAsync(request.FoodTypeId, cancellationToken);
        if (foodType is null)
        {
            return ApiResponse<FoodTypeDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.FoodType),
                HttpStatusCode.NotFound);
        }

        var existingFoodType = await foodTypeRepository.GetByFoodTypeNameAsync(request.FoodTypeName, cancellationToken);
        if (existingFoodType is not null && existingFoodType.FoodTypeId != request.FoodTypeId)
        {
            return ApiResponse<FoodTypeDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.FoodType),
                HttpStatusCode.BadRequest);
        }

        mapper.Map(request, foodType);
        try
        {
            await foodTypeRepository.UpdateAsync(foodType, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<FoodTypeDto>.FailureResponse(
               messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.FoodType), HttpStatusCode.Conflict);
        }

        await cacheService.RemoveAsync($"{CacheKeys.Foodtype}:{request.FoodTypeId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.Foodtype}:", cancellationToken);
        return ApiResponse<FoodTypeDto>.SuccessResponse(
            mapper.Map<FoodTypeDto>(foodType),
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.FoodType), HttpStatusCode.OK);
    }
}