using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class PaymentMode : BaseEntity
{
    public Guid PaymentModeId { get; set; }
    public string PaymentModeName { get; set; } = string.Empty;
}