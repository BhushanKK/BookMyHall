namespace BookMyHall.Contracts.Authentication;

public sealed class GoogleLoginRequest
{
    public string Credential { get; set; } = string.Empty;
    public string DeviceIdentifier { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public string? PushNotificationToken { get; set; }
    public string? AppVersion { get; set; }
}