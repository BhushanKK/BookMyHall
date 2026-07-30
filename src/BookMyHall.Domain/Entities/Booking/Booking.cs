using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Booking;
public class Bookings : BaseEntity
{
    public Guid BookingId { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public Guid HallId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid EventCategoryId { get; set; }
    public Guid HallPricingId { get; set; }
    public DateTimeOffset BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int GuestCount { get; set; }
    public decimal BaseAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public string SpecialRequests { get; set; } = string.Empty;
    public string BookingStatus { get; set; } = string.Empty;
}