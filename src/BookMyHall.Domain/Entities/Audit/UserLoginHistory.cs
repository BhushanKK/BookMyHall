namespace BookMyHall.Domain.Audit;
public class UserLoginHistory
{
    public Guid UserLoginHistoryId { get; set; }
    public Guid UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public DateTimeOffset LoginDate { get; set; }
    public DateTimeOffset? LogoutDate { get; set; }
    public string LoginStatus { get; set; } = string.Empty;
    public string LoginMethod { get; set; }=string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public Guid? SessionId { get; set; }
    public string FailureReason { get; set; } = string.Empty;
    public string LoginSource { get; set; } = string.Empty;
    public bool IsMfaUsed { get; set; } = false;
    public int SessionDurationSeconds { get; set; } = 0;
}