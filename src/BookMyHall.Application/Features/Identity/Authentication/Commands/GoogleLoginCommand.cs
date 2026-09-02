using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Features.Identity.Authentication;

namespace BookMyHall.Application.Features.Authentication;

public sealed record GoogleLoginCommand(
    string Credential,
    string DeviceIdentifier,
    string? DeviceName,
    string? PushNotificationToken,
    string? AppVersion)
    : IRequest<ApiResponse<LoginResponse>>;