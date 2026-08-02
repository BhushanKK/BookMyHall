using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetFoodTypesQueryHandler(
    IFoodTypeRepository foodTypeRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetFoodTypesQuery, ApiResponse<PaginatedResult<FoodType>>>
{
    public async Task<ApiResponse<PaginatedResult<FoodType>>> Handle(GetFoodTypesQuery request,CancellationToken cancellationToken)
    {
        var result = await foodTypeRepository.GetAllAsync(request.paginationRequest,cancellationToken);
        var response = new PaginatedResult<FoodType>
        {
            Items = mapper.Map<IReadOnlyList<FoodType>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<FoodType>>.SuccessResponse( response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.FoodType),HttpStatusCode.OK);
    }
}