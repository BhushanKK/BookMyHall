using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class RegisterDeviceCommandHandler(
    IDeviceRepository deviceRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<RegisterDeviceCommand, ApiResponse<DeviceDto>>
{
    public async Task<ApiResponse<DeviceDto>> Handle(RegisterDeviceCommand request,CancellationToken cancellationToken)
    {
        var existingDevice = await deviceRepository.GetByDeviceIdentifierAsync(request.UserId,request.DeviceIdentifier,cancellationToken);
        if (existingDevice is not null)
        {
            return ApiResponse<DeviceDto>.FailureResponse(messageHelper.AlreadyExists(EntityKeys.Device),HttpStatusCode.BadRequest);
        }
        var device = mapper.Map<Device>(request);
        device.DeviceId = Guid.NewGuid();
        device.IsActive = true;
        device.CreatedDate = DateTimeOffset.UtcNow;
        device.LastLoginDate = DateTimeOffset.UtcNow;
        device.LastActivity = DateTimeOffset.UtcNow;

        await deviceRepository.AddAsync(device, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<DeviceDto>.SuccessResponse(mapper.Map<DeviceDto>(device),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.Device),HttpStatusCode.Created);
    }
}