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

public sealed class CreateEventCategoryCommandHandler(
    IEventCategoryRepository eventCategoryRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateEventCategoryCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreateEventCategoryCommand, ApiResponse<EventCategoryDto>>
{
    public async Task<ApiResponse<EventCategoryDto>> Handle(CreateEventCategoryCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<EventCategoryDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var eventCategory = mapper.Map<EventCategory>(request);
        eventCategory.EventCategoryId = Guid.NewGuid();
        eventCategory.IsActive = true;

        try
        {
            await eventCategoryRepository.AddAsync(eventCategory,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<EventCategoryDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.EventCategory),HttpStatusCode.Conflict);
        }
         await cacheService.RemoveByPrefixAsync($"{CacheKeys.EventCategory}:",cancellationToken);

        return ApiResponse<EventCategoryDto>.SuccessResponse(
            mapper.Map<EventCategoryDto>(eventCategory),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.EventCategory),HttpStatusCode.Created);
    }
}