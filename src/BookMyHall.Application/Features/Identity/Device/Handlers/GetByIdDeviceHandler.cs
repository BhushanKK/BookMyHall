using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetByIdDeviceQueryHandler(
    IDeviceRepository deviceRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<GetByIdDeviceQuery, ApiResponse<DeviceDto>>
{
    public async Task<ApiResponse<DeviceDto>> Handle(
        GetByIdDeviceQuery request,
        CancellationToken cancellationToken)
    {
        var device = await deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken);

        if (device is null)
        {
            return ApiResponse<DeviceDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.Device),
                HttpStatusCode.NotFound
            );
        }

        var deviceDto = mapper.Map<DeviceDto>(device);

        return ApiResponse<DeviceDto>.SuccessResponse
        (
            deviceDto,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Device),
            HttpStatusCode.OK
        );
    }
}