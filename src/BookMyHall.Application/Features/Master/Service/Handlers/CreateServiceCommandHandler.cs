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

public sealed class CreateServiceCommandHandler(IServiceRepository serviceRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<CreateServiceCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreateServiceCommand, ApiResponse<ServiceDto>>
{
    public async Task<ApiResponse<ServiceDto>> Handle(CreateServiceCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<ServiceDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var service = mapper.Map<Service>(request);
        service.ServiceId = Guid.NewGuid();
        service.IsActive = true;

        try
        {
            await serviceRepository.AddAsync(service,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<ServiceDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.Service),HttpStatusCode.Conflict);
        }

        await cacheService.RemoveByPrefixAsync($"{CacheKeys.HallCategory}:", cancellationToken);
        
        return ApiResponse<ServiceDto>.SuccessResponse(
            mapper.Map<ServiceDto>(service),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.Service),HttpStatusCode.Created);
    }
}