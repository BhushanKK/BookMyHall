using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class LoginRequest
{
    public string MobileNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    [JsonIgnore]
    public string DeviceIdentifier { get; set; } = string.Empty;
    [JsonIgnore]
    public string? DeviceName { get; set; }
    [JsonIgnore]
    public string? PushNotificationToken { get; set; }
    [JsonIgnore]
    public string? AppVersion { get; set; }
}