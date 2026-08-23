using System.Net;

using AutoMapper;

using FluentValidation;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateHallCategoryCommandHandler(
    IHallCategoryRepository hallCategoryRepository,
    IUnitOfWork unitOfWork, IMapper mapper,
    IValidator<UpdateHallCategoryCommand> validator,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<UpdateHallCategoryCommand, ApiResponse<HallCategoryDto>>
{
    public async Task<ApiResponse<HallCategoryDto>> Handle(UpdateHallCategoryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<HallCategoryDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var category = await hallCategoryRepository.GetByIdAsync(request.HallCategoryId, cancellationToken);

        if (category is null)
        {
            return ApiResponse<HallCategoryDto>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.HallCategory), HttpStatusCode.NotFound);
        }

        mapper.Map(request, category);

        try
        {
            await hallCategoryRepository.UpdateAsync(category, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<HallCategoryDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.HallCategory), HttpStatusCode.Conflict);
        }
        await cacheService.RemoveAsync($"{CacheKeys.HallCategories}:{request.HallCategoryId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.HallCategoriesPaged}:", cancellationToken);
       
        return ApiResponse<HallCategoryDto>.SuccessResponse(
            mapper.Map<HallCategoryDto>(category),
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.HallCategory), HttpStatusCode.OK);
    }
}