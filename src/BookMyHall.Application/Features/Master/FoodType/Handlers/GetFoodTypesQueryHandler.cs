using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetFoodTypesQueryHandler(
    IFoodTypeRepository foodTypeRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetFoodTypesQuery, ApiResponse<PaginatedResult<FoodTypeDto>>>
{
    public async Task<ApiResponse<PaginatedResult<FoodTypeDto>>> Handle(GetFoodTypesQuery request,CancellationToken cancellationToken)
    {
        var result = await foodTypeRepository.GetAllAsync(request.PaginationRequest,cancellationToken);
        var response = new PaginatedResult<FoodTypeDto>
        {
            Items = mapper.Map<List<FoodTypeDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<FoodTypeDto>>.SuccessResponse( response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.FoodType),HttpStatusCode.OK);
    }
}