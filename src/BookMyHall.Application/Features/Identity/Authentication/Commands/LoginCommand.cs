using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed record LoginCommand(
    string MobileNumber, 
    string Password, 
    string DeviceIdentifier, 
    string? DeviceName, 
    string? PushNotificationToken, 
    string? AppVersion)
    : IRequest<ApiResponse<LoginResponse>>;