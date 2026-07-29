using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class BookingChecklist : BaseEntity
{
    public Guid BookingChecklistId { get; set; }
    public Guid BookingId { get; set; }
    public string ChecklistItem { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public Guid? CompletedBy { get; set; }
    public DateTimeOffset? CompletedDate { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}