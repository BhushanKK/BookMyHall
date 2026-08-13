using System.Net;
using AutoMapper;
using FluentValidation;
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
    IValidator<GetHallCategoryByIdQuery> validator,
    IMessageHelper messageHelper)
    : IRequestHandler< GetHallCategoryByIdQuery,ApiResponse<HallCategoryDto>>
{
    public async Task<ApiResponse<HallCategoryDto>> Handle(GetHallCategoryByIdQuery request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join( " | ",validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<HallCategoryDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

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