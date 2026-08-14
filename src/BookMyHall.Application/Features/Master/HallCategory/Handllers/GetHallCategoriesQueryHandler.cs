using System.Net;
using AutoMapper;
using MediatR;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetHallCategoriesQueryHandler(
    IHallCategoryRepository hallCategoryRepository,
    IMapper mapper,IMessageHelper messageHelper)
    : IRequestHandler<GetHallCategoriesQuery,ApiResponse<PaginatedResponse<HallCategoryDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<HallCategoryDto>>> Handle(GetHallCategoriesQuery request,CancellationToken cancellationToken)
    {
        var pagedResult = await hallCategoryRepository.GetAllAsync(request.Request,cancellationToken);
        var response = new PaginatedResponse<HallCategoryDto>
        {
            Items = mapper.Map<IReadOnlyList<HallCategoryDto>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        return ApiResponse<PaginatedResponse<HallCategoryDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.HallCategory),HttpStatusCode.OK);
    }
}