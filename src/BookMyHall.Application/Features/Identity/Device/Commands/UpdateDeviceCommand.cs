using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateDeviceCommand() 
:DeviceDto, IRequest<ApiResponse<DeviceDto>>;

    