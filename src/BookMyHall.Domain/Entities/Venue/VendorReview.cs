using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class VendorReview : BaseEntity
{
    public Guid VendorReviewId { get; set; }
    public Guid VendorId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? VendorBookingId { get; set; }
    public short Rating { get; set; }
    public string? Title { get; set; }
    public string? ReviewText { get; set; }
    public string? VendorResponse { get; set; }
    public DateTime? VendorResponseDate { get; set; }
    public bool IsVerifiedBooking { get; set; }
    public string ModerationStatus { get; set; } = "Pending";
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public virtual Vendors Vendor { get; set; } = null!;
}
