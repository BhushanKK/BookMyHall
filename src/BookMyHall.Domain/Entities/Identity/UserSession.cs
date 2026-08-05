using BookMyHall.Domain.Common;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Domain.Identity;
public class UserSession : BaseEntity
{
    public Guid UserSessionId { get; set; }
    public Guid UserId { get; set; }
    public Guid RefreshTokenId { get; set; }
    public Guid? DeviceId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTimeOffset SessionStart { get; set; }
    public DateTimeOffset LastActivity { get; set; }
    public DateTimeOffset? SessionEnd { get; set; }
    public bool IsActive { get; set; } = true;
    public string? LogoutReason { get; set; }
    public User User { get; set; } = default!;
    public RefreshToken RefreshToken { get; set; } = default!;
    public Device? Device { get; set; }
}