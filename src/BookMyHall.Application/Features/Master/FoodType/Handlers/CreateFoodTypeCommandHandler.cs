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

namespace BookMyHall.Application.Features.Master;

public sealed class CreateFoodTypeCommandHandler(
    IFoodTypeRepository foodTypeRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateFoodTypeCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<CreateFoodTypeCommand, ApiResponse<FoodTypeDto>>
{
    public async Task<ApiResponse<FoodTypeDto>> Handle(CreateFoodTypeCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<FoodTypeDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var foodType = mapper.Map<FoodType>(request);
        foodType.FoodTypeId = Guid.NewGuid();
        foodType.IsActive = true;

        try
        {
            await foodTypeRepository.AddAsync(foodType,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<FoodTypeDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.FoodType),HttpStatusCode.Conflict);
        }

        return ApiResponse<FoodTypeDto>.SuccessResponse(
            mapper.Map<FoodTypeDto>(foodType),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.FoodType),HttpStatusCode.Created);
    }
}