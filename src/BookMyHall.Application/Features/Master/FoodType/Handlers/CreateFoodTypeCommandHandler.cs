using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateFoodTypeCommandHandler(
    IFoodTypeRepository foodTypeRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<CreateFoodTypeCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateFoodTypeCommand request,CancellationToken cancellationToken)
    {
        var existingFoodType = await foodTypeRepository.GetByFoodTypeNameAsync(request.FoodTypeName,cancellationToken);
        if (existingFoodType is not null)
        {
            return ApiResponse<Guid>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.FoodType),
                HttpStatusCode.BadRequest);
        }

        var foodType = mapper.Map<FoodType>(request);
        foodType.FoodTypeId = Guid.NewGuid();
        foodType.IsActive = true;
        await foodTypeRepository.AddAsync(foodType, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<Guid>.SuccessResponse(foodType.FoodTypeId,
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.FoodType),HttpStatusCode.Created);
    }
}