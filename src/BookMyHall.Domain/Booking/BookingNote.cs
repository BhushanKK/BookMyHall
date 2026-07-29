using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class BookingNote : BaseEntity
{
    public Guid BookingNoteId { get; set; }
    public Guid BookingId { get; set; }
    public string Note { get; set; } = string.Empty;
    public string NoteType { get; set; } = string.Empty;
    public bool  IsImportant { get; set; }
}