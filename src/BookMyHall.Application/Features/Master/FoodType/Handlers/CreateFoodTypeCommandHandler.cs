using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;
public sealed class CreateFoodTypeCommandHandler(IFoodTypeRepository foodTypeRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<CreateFoodTypeCommand> validator,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<CreateFoodTypeCommand, ApiResponse<FoodTypeDto>>
{
    public async Task<ApiResponse<FoodTypeDto>> Handle(CreateFoodTypeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<FoodTypeDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var foodTypeName = request.FoodTypeName.Trim();
        var existingFoodType =await foodTypeRepository.GetByNameIncludingDeletedAsync(foodTypeName,cancellationToken);

        // Active record already exists.
        if (existingFoodType is not null && !existingFoodType.IsDeleted)
        {
            return ApiResponse<FoodTypeDto>.FailureResponse(messageHelper.AlreadyExistsEntity
            (ResourceNames.Entities,EntityKeys.FoodType),HttpStatusCode.Conflict);
        }

        // Restore previously soft-deleted record.
        if (existingFoodType is not null &&
            existingFoodType.IsDeleted)
        {
            existingFoodType.IsDeleted = false;
            existingFoodType.IsActive = true;
            existingFoodType.FoodTypeName =foodTypeName;

            await foodTypeRepository.UpdateAsync(existingFoodType,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateCacheAsync(existingFoodType.FoodTypeId,cancellationToken);

            return ApiResponse<FoodTypeDto>.SuccessResponse(mapper.Map<FoodTypeDto>(existingFoodType),
                messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.FoodType),HttpStatusCode.Created);
        }

        // Create a completely new record.
        var foodType = mapper.Map<FoodType>(request);
        foodType.FoodTypeId = Guid.NewGuid();
        foodType.FoodTypeName = foodTypeName;
        foodType.IsActive = true;
        foodType.IsDeleted = false;

        try
        {
            await foodTypeRepository.AddAsync(foodType,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<FoodTypeDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,
                    EntityKeys.FoodType),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(foodType.FoodTypeId,cancellationToken);
        return ApiResponse<FoodTypeDto>.SuccessResponse(mapper.Map<FoodTypeDto>(foodType),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.FoodType),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid foodTypeId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.Foodtype}:{foodTypeId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.FoodtypePaged}:",cancellationToken);
    }
}