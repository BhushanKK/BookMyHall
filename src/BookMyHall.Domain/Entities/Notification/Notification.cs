using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Notifications;
public class Notification : BaseEntity
{
    public Guid NotificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; }= string.Empty;
    public DateTimeOffset ScheduledDate { get; set; }
}