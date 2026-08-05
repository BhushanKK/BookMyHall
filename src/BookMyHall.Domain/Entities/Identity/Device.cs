using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Domain.Identity;
public class Device
{
    public Guid DeviceId { get; set; }
    public Guid UserId { get; set; }
    public string DeviceIdentifier { get; set; } = string.Empty;
    public string? PushNotificationToken { get; set; }
    public string? DeviceName { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string? OperatingSystem { get; set; }
    public string? Browser { get; set; }
    public string? AppVersion { get; set; }
    public string? LastIpAddress { get; set; }
    public DateTimeOffset LastLoginDate { get; set; }
    public DateTimeOffset? LastActivity { get; set; }
    public bool IsTrusted { get; set; } = false;
    public DateTimeOffset? TrustedDate { get; set; }
    public bool IsActive { get; set; } 
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public User User { get; set; } = default!;
}