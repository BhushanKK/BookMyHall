using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateDeviceCommandHandler(
    IDeviceRepository deviceRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper,ICacheService cacheService)
    : IRequestHandler<UpdateDeviceCommand, ApiResponse<DeviceDto>>
{
    public async Task<ApiResponse<DeviceDto>> Handle(UpdateDeviceCommand request,CancellationToken cancellationToken)
    {
        var device = await deviceRepository.GetByDeviceIdentifierAsync(request.UserId,request.DeviceIdentifier,cancellationToken);

        if (device is null)
        {
            return ApiResponse<DeviceDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Device),
                HttpStatusCode.NotFound);
        }

        mapper.Map(request, device);
        device.UpdatedDate = DateTimeOffset.UtcNow;
        device.LastActivity = DateTimeOffset.UtcNow;
        await deviceRepository.UpdateAsync(device, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Devices}:{request.DeviceId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync(CacheKeys.DevicePaged, cancellationToken);
        return ApiResponse<DeviceDto>.SuccessResponse(mapper.Map<DeviceDto>(device),
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.Device),HttpStatusCode.OK);
    }
}