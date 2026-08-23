using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class DeleteServiceCommandHandler(IServiceRepository serviceRepository,
    IUnitOfWork unitOfWork,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<DeleteServiceCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteServiceCommand request,CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetByIdAsync(request.ServiceId,cancellationToken);

        if (service is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Service),
                HttpStatusCode.NotFound);
        }
        service.IsActive = false;
        await serviceRepository.UpdateAsync(service, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Services}:{request.ServiceId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.ServicesPaged}:", cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities,EntityKeys.Service),HttpStatusCode.OK);
    }
}