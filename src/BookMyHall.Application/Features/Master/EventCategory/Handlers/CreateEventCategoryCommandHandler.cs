using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateEventCategoryCommandHandler(IEventCategoryRepository eventCategoryRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<CreateEventCategoryCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreateEventCategoryCommand,ApiResponse<EventCategoryDto>>
{
    public async Task<ApiResponse<EventCategoryDto>> Handle(CreateEventCategoryCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<EventCategoryDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var eventCategoryName = request.EventCategoryName.Trim();
        var existingEventCategory =await eventCategoryRepository.GetByNameIncludingDeletedAsync(eventCategoryName,cancellationToken);

        // Active record already exists.
        if (existingEventCategory is not null && !existingEventCategory.IsDeleted)
        {
            return ApiResponse<EventCategoryDto>.FailureResponse(messageHelper.AlreadyExistsEntity
            (ResourceNames.Entities,EntityKeys.EventCategory),HttpStatusCode.Conflict);
        }

        // Restore previously soft-deleted record.
        if (existingEventCategory is not null &&
            existingEventCategory.IsDeleted)
        {
            existingEventCategory.IsDeleted = false;
            existingEventCategory.IsActive = true;
            existingEventCategory.EventCategoryName =eventCategoryName;

            await eventCategoryRepository.UpdateAsync(existingEventCategory,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateCacheAsync(existingEventCategory.EventCategoryId,cancellationToken);

            return ApiResponse<EventCategoryDto>.SuccessResponse(mapper.Map<EventCategoryDto>(existingEventCategory),
                messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.EventCategory),HttpStatusCode.Created);
        }

        // Create a completely new record.
        var eventCategory = mapper.Map<EventCategory>(request);
        eventCategory.EventCategoryId = Guid.NewGuid();
        eventCategory.EventCategoryName = eventCategoryName;
        eventCategory.IsActive = true;
        eventCategory.IsDeleted = false;

        try
        {
            await eventCategoryRepository.AddAsync(eventCategory,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<EventCategoryDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,
                    EntityKeys.EventCategory),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(eventCategory.EventCategoryId,cancellationToken);
        return ApiResponse<EventCategoryDto>.SuccessResponse(mapper.Map<EventCategoryDto>(eventCategory),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.EventCategory),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid eventCategoryId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.EventCategories}:{eventCategoryId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.EventCategoriesPaged}:",cancellationToken);
    }
}