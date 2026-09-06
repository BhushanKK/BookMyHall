using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class RegisterDeviceCommandHandler(
    IDeviceRepository deviceRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper,
    ICacheService cacheService,
    ICurrentUser currentUser)
    : IRequestHandler<RegisterDeviceCommand, ApiResponse<DeviceDto>>
{
    public async Task<ApiResponse<DeviceDto>> Handle(
        RegisterDeviceCommand request,
        CancellationToken cancellationToken)
    {
         if (!currentUser.UserId.HasValue)
        {
            return ApiResponse<DeviceDto>.FailureResponse
            (
                "User authentication is required.",
                HttpStatusCode.Unauthorized
            );
        }
        var userId = currentUser.UserId;

        var existingDevice =await deviceRepository.GetByDeviceIdentifierAsync(
                userId.Value,
                request.DeviceIdentifier,
                cancellationToken);

        if (existingDevice is not null)
        {
            return ApiResponse<DeviceDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.Device),
                HttpStatusCode.BadRequest);
        }

        var device = mapper.Map<Device>(request);

        device.DeviceId = Guid.NewGuid();
        device.UserId = userId.Value;
        device.IsActive = true;
        device.CreatedDate = DateTimeOffset.UtcNow;
        device.LastLoginDate = DateTimeOffset.UtcNow;
        device.LastActivity = DateTimeOffset.UtcNow;

        await deviceRepository.AddAsync(
            device,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        await cacheService.RemoveByPrefixAsync(
            CacheKeys.DevicePaged,
            cancellationToken);

        return ApiResponse<DeviceDto>.SuccessResponse(
            mapper.Map<DeviceDto>(device),
            messageHelper.AddedEntity(
                ResourceNames.Entities,
                EntityKeys.Device),
            HttpStatusCode.Created);
    }
}