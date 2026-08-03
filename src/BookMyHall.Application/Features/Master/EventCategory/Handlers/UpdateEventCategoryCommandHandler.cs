using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateEventCategoryCommandHandler(
    IEventCategoryRepository eventCategoryRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateEventCategoryCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<UpdateEventCategoryCommand, ApiResponse<EventCategoryDto>>
{
    public async Task<ApiResponse<EventCategoryDto>> Handle(UpdateEventCategoryCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<EventCategoryDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var eventCategory = await eventCategoryRepository.GetByIdAsync(request.EventCategoryId,cancellationToken);

        if (eventCategory is null)
        {
            return ApiResponse<EventCategoryDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.EventCategory),
                HttpStatusCode.NotFound);
        }

        var existingEventCategory = await eventCategoryRepository.GetByEventCategoryNameAsync(request.EventCategoryName,cancellationToken);
        if (existingEventCategory is not null && existingEventCategory.EventCategoryId != request.EventCategoryId)
        {
            return ApiResponse<EventCategoryDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.EventCategory),
                HttpStatusCode.BadRequest);
        }

        mapper.Map(request, eventCategory);
        await eventCategoryRepository.UpdateAsync(eventCategory,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<EventCategoryDto>.SuccessResponse(
            mapper.Map<EventCategoryDto>(eventCategory),
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.EventCategory),HttpStatusCode.OK);
    }
}