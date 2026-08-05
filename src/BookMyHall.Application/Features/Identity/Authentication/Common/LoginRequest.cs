namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class LoginRequest
{
    public string MobileNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DeviceIdentifier { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public string? PushNotificationToken { get; set; }
    public string? AppVersion { get; set; }
}