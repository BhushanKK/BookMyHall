using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class Refund : BaseEntity
{
    public Guid RefundId { get; set; }
    public Guid PaymentId { get; set; }
    public string RefundNumber { get; set; } = string.Empty;
    public DateTimeOffset RefundDate { get; set; }
    public decimal RefundAmount { get; set; }
    public string RefundReason { get; set; } = string.Empty;
    public string GatewayRefundId { get; set; } = string.Empty;
    public string RefundStatus { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
}