using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetDeviceQueryHandler(
    IDeviceRepository deviceRepository,
    IMapper mapper,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<GetDeviceQuery, ApiResponse<PaginatedResponse<DeviceDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<DeviceDto>>> Handle(
        GetDeviceQuery request,
        CancellationToken cancellationToken)
    {
         var pagination = request.paginationRequest;

        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<DeviceDto>(
            CacheKeys.Devices,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<DeviceDto>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<DeviceDto>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Device),
                HttpStatusCode.OK
            );
        }
        var pagedResult = await deviceRepository.GetAllAsync(request.paginationRequest, cancellationToken);

        var response = new PaginatedResponse<DeviceDto>
        {
            Items = mapper.Map<IReadOnlyList<DeviceDto>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<DeviceDto>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Device),
            HttpStatusCode.OK
        );
    }
}