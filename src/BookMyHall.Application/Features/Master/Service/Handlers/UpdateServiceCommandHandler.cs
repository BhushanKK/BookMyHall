using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateServiceCommandHandler(IServiceRepository serviceRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<UpdateServiceCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<UpdateServiceCommand, ApiResponse<ServiceDto>>
{
    public async Task<ApiResponse<ServiceDto>> Handle(UpdateServiceCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<ServiceDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var service = await serviceRepository.GetByIdAsync(request.ServiceId,cancellationToken);
        if (service is null)
        {
            return ApiResponse<ServiceDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Service),
                HttpStatusCode.NotFound);
        }

        var existingService = await serviceRepository.GetByServiceNameAsync(request.ServiceName,cancellationToken);
        if (existingService is not null && existingService.ServiceId != request.ServiceId)
        {
            return ApiResponse<ServiceDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.Service),
                HttpStatusCode.BadRequest);
        }

        mapper.Map(request, service);
        await serviceRepository.UpdateAsync(service,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Service}:{request.ServiceId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.Service}:", cancellationToken);
        return ApiResponse<ServiceDto>.SuccessResponse(
            mapper.Map<ServiceDto>(service),
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.Service),HttpStatusCode.OK);
    }
}