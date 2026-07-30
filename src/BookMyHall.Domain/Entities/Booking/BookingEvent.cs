using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class BookingEvent:BaseEntity
{
    public Guid BookingEventId { get; set; }
    public Guid BookingId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string BrideName { get; set; } = string.Empty;
    public string GroomName { get; set; } = string.Empty;
    public string BirthdayPersonName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public string EventDescription { get; set; } = string.Empty;
    
}