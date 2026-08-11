using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class DeleteDeviceCommandHandler(
    IDeviceRepository deviceRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper)
    : IRequestHandler<DeleteDeviceCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteDeviceCommand request,CancellationToken cancellationToken)
    {
        var device = await deviceRepository.GetByDeviceIdentifierAsync(request.UserId,request.DeviceIdentifier,cancellationToken);

        if (device is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFound(EntityKeys.Device),HttpStatusCode.NotFound);
        }

        device.IsActive = false;
        device.UpdatedDate = DateTimeOffset.UtcNow;
        await deviceRepository.UpdateAsync(device, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities,EntityKeys.Device),HttpStatusCode.OK);
    }
}