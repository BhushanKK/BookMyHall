using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed class RegisterDeviceCommand :DeviceDto, IRequest<ApiResponse<DeviceDto>>;
