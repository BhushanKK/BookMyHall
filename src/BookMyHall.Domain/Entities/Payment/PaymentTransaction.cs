using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Payments;
public class PaymentTransaction : BaseEntity
{
    public Guid PaymentTransactionId { get; set; }
    public Guid PaymentId { get; set; }
    public string TransactionNumber { get; set; } = string.Empty;
    public string GatewayName { get; set; } = string.Empty;
    public string GatewayTransactionId { get; set; } = string.Empty;
    public string GatewayOrderId { get; set; } = string.Empty;
    public string GatewayPaymentId { get; set; } = string.Empty;
    public decimal TransactionAmount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string TransactionStatus { get; set; } = string.Empty;
    public string GatewayResponse { get; set; } = string.Empty;
    public DateTimeOffset TransactionDate { get; set; }
}