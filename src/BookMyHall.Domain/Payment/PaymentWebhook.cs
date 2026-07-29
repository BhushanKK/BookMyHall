using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class PaymentWebhook : BaseEntity
{
    public Guid PaymentWebhookId { get; set; }
    public string GatewayName { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string GatewayEventId { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
}