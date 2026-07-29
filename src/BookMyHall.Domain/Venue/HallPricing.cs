using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class HallPricing : BaseEntity
{
    public Guid HallPricingId { get; set; }
    public Guid HallId { get; set; }
    public Guid EventCategoryId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string MinimumGuests { get; set; } = string.Empty;
    public string MaximumGuests { get; set; } = string.Empty;
    public decimal WeekdayPrice { get; set; } = 0;
    public decimal WeekendPrice { get; set; } = 0;
    public decimal AdvanceAmount { get; set; } = 0;
    public decimal SecurityDeposit { get; set; } = 0;
    public decimal ExtraGuestCharge { get; set; } = 0;    
}