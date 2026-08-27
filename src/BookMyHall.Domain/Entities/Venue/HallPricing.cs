using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class HallPricing : BaseEntity
{
    public Guid HallPricingId { get; set; }
    public Guid HallId { get; set; }
    public Guid EventCategoryId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public int? MinimumGuests { get; set; }
    public int? MaximumGuests { get; set; }
    public decimal? WeekdayPrice { get; set; }
    public decimal? WeekendPrice { get; set; }
    public decimal? AdvanceAmount { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public decimal? ExtraGuestCharge { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
}