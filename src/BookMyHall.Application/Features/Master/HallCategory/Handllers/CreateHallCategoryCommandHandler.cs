using System.Net;

using AutoMapper;

using FluentValidation;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateHallCategoryCommandHandler(
    IHallCategoryRepository hallCategoryRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateHallCategoryCommand> validator,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<CreateHallCategoryCommand, ApiResponse<HallCategoryDto>>
{
    public async Task<ApiResponse<HallCategoryDto>> Handle(CreateHallCategoryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<HallCategoryDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var category = mapper.Map<HallCategory>(request);

        try
        {
            await hallCategoryRepository.AddAsync(category, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<HallCategoryDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.HallCategory), HttpStatusCode.Conflict);
        }
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.HallCategoriesPaged}:", cancellationToken);
        return ApiResponse<HallCategoryDto>.SuccessResponse(
            mapper.Map<HallCategoryDto>(category),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.HallCategory), HttpStatusCode.Created);
    }
}