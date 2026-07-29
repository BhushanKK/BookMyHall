using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;

public class NotificationQueue : BaseEntity
{
    public Guid NotificationQueueId { get; set; }
    public Guid NotificationId { get; set; }
    public Guid NotificationRecipientId { get; set; }
    public Guid NotificationTemplateId { get; set; }
    public DateTimeOffset ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int RetryCount { get; set; }
    public int MaxRetryCount { get; set; }
    public DateTimeOffset? LastAttemptDate { get; set; }
    public DateTimeOffset? ProcessedDate { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}