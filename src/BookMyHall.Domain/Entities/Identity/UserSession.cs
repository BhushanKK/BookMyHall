using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class UserSession : BaseEntity
{
    public Guid UserSessionId { get; set; }
    public Guid UserId { get; set; }
    public Guid RefreshTokenId { get; set; } 
    public Guid DeviceId { get; set; }      
    public TimeSpan SessionStart { get; set; }
    public TimeSpan SessionEnd { get; set; }
    public TimeSpan LastActivity  { get; set; }
    public bool IsActive { get; set; } 
}   