using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Notifications;
public class NotificationRecipient : BaseEntity
{
    public Guid NotificationRecipientId { get; set; }
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string RecipientMobile { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
    public string RecipientType { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTimeOffset ReadDate { get; set; }
}