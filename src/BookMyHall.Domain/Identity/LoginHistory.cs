using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class LoginHistory : BaseEntity
{
    public Guid LoginHistoryId { get; set; }
    public Guid UserId { get; set; }
    public string IPAddress { get; set; } = string.Empty;
    public string OpratingSystem { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
    public DateTimeOffset LoginTime { get; set; }
    public string Browser { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public string FailureReason { get; set; } = string.Empty;
}