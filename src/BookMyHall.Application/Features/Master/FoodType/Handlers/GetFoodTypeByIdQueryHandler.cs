using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetFoodTypeByIdQueryHandler(
    IFoodTypeRepository foodTypeRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetFoodTypeByIdQuery, ApiResponse<FoodTypeDto>>
{
    public async Task<ApiResponse<FoodTypeDto>> Handle(GetFoodTypeByIdQuery request,CancellationToken cancellationToken)
    {
        var foodType = await foodTypeRepository.GetByIdAsync(request.FoodTypeId,cancellationToken);
        if (foodType is null)
        {
            return ApiResponse<FoodTypeDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.FoodType),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<FoodTypeDto>.SuccessResponse(
            mapper.Map<FoodTypeDto>(foodType),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.FoodType),HttpStatusCode.OK);
    }
}