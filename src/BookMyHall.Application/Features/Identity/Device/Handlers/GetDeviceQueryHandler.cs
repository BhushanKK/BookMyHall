using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetDeviceQueryHandler(
    IDeviceRepository deviceRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<GetDeviceQuery, ApiResponse<PaginatedResponse<DeviceDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<DeviceDto>>> Handle(
        GetDeviceQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await deviceRepository.GetAllAsync(request.Request, cancellationToken);

        var response = new PaginatedResponse<DeviceDto>
        {
            Items = mapper.Map<IReadOnlyList<DeviceDto>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        return ApiResponse<PaginatedResponse<DeviceDto>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Device),
            HttpStatusCode.OK
        );
    }
}