using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class BookingDocument : BaseEntity
{
    public Guid BookingDocumentId { get; set; }
    public Guid BookingId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public Guid VerifiedBy { get; set; }
    public DateTimeOffset VerifiedDate { get; set; }
}