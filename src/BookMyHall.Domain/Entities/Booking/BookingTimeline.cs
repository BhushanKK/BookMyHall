using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;        
public class BookingTimeline : BaseEntity
{
    public Guid BookingTimelineId { get; set; }
    public Guid BookingId { get; set; }
    public string ActivityType { get; set; } = string.Empty;        
    public string ActivityTitle { get; set; } = string.Empty;   
    public string ActivityDescription { get; set; } = string.Empty;
    public  Guid PerformedBy { get; set; }
    public string PerformedByType { get; set; } = string.Empty; 
    public Guid ReferenceId { get; set; } 
    public string ReferenceType { get; set; } = string.Empty;
    public DateTimeOffset ActivityDate { get; set; }
}
