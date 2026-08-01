using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateFoodTypeCommandHandler(
    IFoodTypeRepository foodTypeRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<UpdateFoodTypeCommand, ApiResponse<FoodTypeDto>>
{
    public async Task<ApiResponse<FoodTypeDto>> Handle(UpdateFoodTypeCommand request,CancellationToken cancellationToken)
    {
        var foodType = await foodTypeRepository.GetByIdAsync(request.FoodTypeId,cancellationToken);
        if (foodType is null)
        {
            return ApiResponse<FoodTypeDto>.FailureResponse(messageHelper.NotFound(EntityKeys.FoodType),HttpStatusCode.NotFound);
        }
        var existingFoodType = await foodTypeRepository.GetByFoodTypeNameAsync(request.FoodTypeName,cancellationToken);
        if (existingFoodType is not null && existingFoodType.FoodTypeId != request.FoodTypeId)
        {
            return ApiResponse<FoodTypeDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.FoodType),
                HttpStatusCode.BadRequest);
        }

        mapper.Map(request, foodType);
        await foodTypeRepository.UpdateAsync(foodType, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        var foodTypeDto = mapper.Map<FoodTypeDto>(foodType);
        return ApiResponse<FoodTypeDto>.SuccessResponse(foodTypeDto,
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.FoodType),HttpStatusCode.OK);
    }
}