using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class BookingReminder : BaseEntity
{
    public Guid BookingReminderId { get; set; }
    public Guid BookingId { get; set; }
    public string ReminderType { get; set; } = string.Empty;
    public string ReminderTitle { get; set; } = string.Empty;
    public string ReminderMessage { get; set; } = string.Empty;
    public DateTimeOffset ReminderDate { get; set; }
    public string NotificationChannel { get; set; } = string.Empty;
    public string ReminderStatus { get; set; } = string.Empty;
    public DateTimeOffset? SentDate { get; set; }
}