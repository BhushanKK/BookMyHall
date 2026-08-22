using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetByIdDeviceQueryHandler(
    IDeviceRepository deviceRepository,
    IMapper mapper,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetByIdDeviceQuery, ApiResponse<DeviceDto>>
{
    public async Task<ApiResponse<DeviceDto>> Handle(
        GetByIdDeviceQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.Devices}:{request.DeviceId}";

        var cachedDevice = await cacheService.GetAsync<DeviceDto>(cacheKey, cancellationToken);

        if (cachedDevice is not null)
        {
            return ApiResponse<DeviceDto>.SuccessResponse
            (
                cachedDevice,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Device),
                HttpStatusCode.OK
            );
        }
        var device = await deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken);

        if (device is null)
        {
            return ApiResponse<DeviceDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.Device),
                HttpStatusCode.NotFound
            );
        }

        var response = mapper.Map<DeviceDto>(device);
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return ApiResponse<DeviceDto>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Device),
            HttpStatusCode.OK
        );
    }
}