using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Booking;
public class BookingCancellation : BaseEntity
{
    public Guid BookingCancellationId { get; set; }
    public Guid BookingId { get; set; }
    public Guid CancellationBy { get; set; }
    public DateTimeOffset CancellationDate { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
    public decimal CancellationCharge { get; set; }
    public decimal RefundAmount { get; set; }
    public Guid RefundId { get; set; }
    public string Remarks { get; set; } = string.Empty;
}