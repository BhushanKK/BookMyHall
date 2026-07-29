using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class HallReview : BaseEntity
{
    public Guid HallReviewId { get; set; }
    public Guid HallId { get; set; }
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public int Rating { get; set; }
    public string ReviewTitle { get; set; } = string.Empty;
    public string Review { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
}