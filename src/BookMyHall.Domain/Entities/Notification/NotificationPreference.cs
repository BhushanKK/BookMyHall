using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Notifications;
public class NotificationPreference : BaseEntity
{
    public Guid NotificationPreferenceId { get; set; }
    public Guid UserId { get; set; }
    public bool IsEmailEnabled { get; set; }
    public bool IsSmsEnabled { get; set; }
    public bool IsPushEnabled { get; set; }
    public bool IsInAppEnabled { get; set; }
    public bool IsWhatsAppEnabled { get; set; }
}