using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetFoodTypeByIdQueryHandler(
    IFoodTypeRepository foodTypeRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetFoodTypeByIdQuery, ApiResponse<FoodType>>
{
    public async Task<ApiResponse<FoodType>> Handle(GetFoodTypeByIdQuery request,CancellationToken cancellationToken)
    {
        var foodType = await foodTypeRepository.GetByIdAsync(request.FoodTypeId,cancellationToken);
        if (foodType is null)
        {
            return ApiResponse<FoodType>.FailureResponse(
                messageHelper.NotFound(EntityKeys.FoodType),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<FoodType>.SuccessResponse(
            mapper.Map<FoodType>(foodType),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.FoodType),HttpStatusCode.OK);
    }
}