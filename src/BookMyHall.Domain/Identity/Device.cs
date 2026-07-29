using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class Device : BaseEntity
{
    public Guid DeviceId { get; set; }
    public Guid UserId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
     public string DeviceType { get; set; } = string.Empty;
     public string OperatingSystem { get; set; } = string.Empty;
     public string AppVersion { get; set; } = string.Empty;
}