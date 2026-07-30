using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Audit;
public class UserLoginHistory : BaseEntity
{
    public Guid UserLoginHistoryId { get; set; }
    public Guid UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public DateTimeOffset LoginDate { get; set; }
    public DateTimeOffset? LogoutDate { get; set; }
    public string LoginStatus { get; set; } = string.Empty;
    public string LoginMethod { get; set; }=string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string DeviceeType { get; set; } = string.Empty;
    public string OpratingSystem { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public Guid SeesionId { get; set; }
    public string FailureReason { get; set; } = string.Empty;
}