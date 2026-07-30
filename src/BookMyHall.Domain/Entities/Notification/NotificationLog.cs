using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Notifications;
public class NotificationLog : BaseEntity
{
    public Guid NotificationLogId { get; set; }
    public Guid NotificationQueueId { get; set; }
    public Guid NotificationId { get; set; }
    public Guid NotificationRecipientId { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public Guid NotificationProviderId { get; set; }
    public string ProviderMessageId { get; set; }= string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ResponseCode { get; set; } = string.Empty;
    public string ResponseMessage { get; set; } = string.Empty;
    public DateTimeOffset? SentDate { get; set; }
    public DateTimeOffset? DeliveredDate { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}