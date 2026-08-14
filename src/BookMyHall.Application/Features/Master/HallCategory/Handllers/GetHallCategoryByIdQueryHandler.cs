using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetHallCategoryByIdQueryHandler(
    IHallCategoryRepository hallCategoryRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler< GetHallCategoryByIdQuery,ApiResponse<HallCategoryDto>>
{
    public async Task<ApiResponse<HallCategoryDto>> Handle(GetHallCategoryByIdQuery request,CancellationToken cancellationToken)
    {
        var category = await hallCategoryRepository.GetByIdAsync(request.HallCategoryId,cancellationToken);

        if (category is null)
        {
            return ApiResponse<HallCategoryDto>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.HallCategory),HttpStatusCode.NotFound);
        }

        return ApiResponse<HallCategoryDto>.SuccessResponse(
            mapper.Map<HallCategoryDto>(category),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.HallCategory), HttpStatusCode.OK);
    }
}