using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
namespace BookMyHall.Application.Features.Master;
public sealed class CreateEventCategoryCommandHandler(
    IEventCategoryRepository eventCategoryRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<CreateEventCategoryCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateEventCategoryCommand request,CancellationToken cancellationToken)
    {
        var existingEventCategory = await eventCategoryRepository
            .GetByEventCategoryNameAsync(request.EventCategoryName,cancellationToken);

        if (existingEventCategory is not null)
        {
            return ApiResponse<Guid>.FailureResponse(messageHelper.AlreadyExists(EntityKeys.EventCategory),HttpStatusCode.BadRequest);
        }

        var eventCategory = mapper.Map<EventCategory>(request);
        eventCategory.EventCategoryId = Guid.NewGuid();
        eventCategory.IsActive = true;
        await eventCategoryRepository.AddAsync(eventCategory,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.SuccessResponse(eventCategory.EventCategoryId,
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.EventCategory),HttpStatusCode.Created);
    }
}