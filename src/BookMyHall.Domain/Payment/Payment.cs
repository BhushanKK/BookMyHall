using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class Payment : BaseEntity
{
    public Guid PaymentId { get; set; }
    public Guid BookingId { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public DateTimeOffset PaymentDate { get; set; }
    public Guid PaymentModeId { get; set; }
    public decimal Amount { get; set; }
    public string GatewayTransactionId { get; set; } = string.Empty;
    public string GatewayReferenceNumber { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
}