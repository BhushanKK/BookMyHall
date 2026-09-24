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
public sealed class CreateHallCategoryCommandHandler(IHallCategoryRepository hallCategoryRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<CreateHallCategoryCommand> validator,
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

        var hallCategoryName = request.HallCategoryName.Trim();
        var existingHallCategory =await hallCategoryRepository.GetByNameIncludingDeletedAsync(hallCategoryName,cancellationToken);

        // Active record already exists.
        if (existingHallCategory is not null && !existingHallCategory.IsDeleted)
        {
            return ApiResponse<HallCategoryDto>.FailureResponse(messageHelper.AlreadyExistsEntity
            (ResourceNames.Entities,EntityKeys.HallCategory),HttpStatusCode.Conflict);
        }

        // Restore previously soft-deleted record.
        if (existingHallCategory is not null &&
            existingHallCategory.IsDeleted)
        {
            existingHallCategory.IsDeleted = false;
            existingHallCategory.IsActive = true;
            existingHallCategory.HallCategoryName =hallCategoryName;

            await hallCategoryRepository.UpdateAsync(existingHallCategory,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateCacheAsync(existingHallCategory.HallCategoryId,cancellationToken);

            return ApiResponse<HallCategoryDto>.SuccessResponse(mapper.Map<HallCategoryDto>(existingHallCategory),
                messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.HallCategory),HttpStatusCode.Created);
        }

        // Create a completely new record.
        var hallCategory = mapper.Map<HallCategory>(request);
        hallCategory.HallCategoryId = Guid.NewGuid();
        hallCategory.HallCategoryName = hallCategoryName;
        hallCategory.IsActive = true;
        hallCategory.IsDeleted = false;

        try
        {
            await hallCategoryRepository.AddAsync(hallCategory,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<HallCategoryDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,
                    EntityKeys.HallCategory),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(hallCategory.HallCategoryId,cancellationToken);
        return ApiResponse<HallCategoryDto>.SuccessResponse(mapper.Map<HallCategoryDto>(hallCategory),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.HallCategory),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid hallCategoryId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.HallCategories}:{hallCategoryId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.HallCategoriesPaged}:",cancellationToken);
    }
}