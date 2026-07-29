using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class NotificationProvider : BaseEntity
{
    public Guid NotificationProviderId { get; set; }
    public string ProviderCode { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public int Priority { get; set; }
}