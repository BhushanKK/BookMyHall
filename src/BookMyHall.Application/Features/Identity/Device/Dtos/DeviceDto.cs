using System.Text.Json.Serialization;
namespace BookMyHall.Application.Features.Identity;

public class DeviceDto
{
    [JsonIgnore]
    public Guid DeviceId { get; set; }
    [JsonIgnore]
    public Guid UserId { get; set; }
    public string DeviceIdentifier { get; set; } = string.Empty;
    public string PushNotificationToken { get; set; }= string.Empty;
    public string DeviceName { get; set; }= string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string OperatingSystem { get; set; }= string.Empty;
    public string Browser { get; set; }= string.Empty;
    public string AppVersion { get; set; }= string.Empty;
    public string LastIpAddress { get; set; }= string.Empty;
    public DateTimeOffset LastLoginDate { get; set; }
    public DateTimeOffset? LastActivity { get; set; }
    public bool IsTrusted { get; set; }
    public DateTimeOffset? TrustedDate { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
}