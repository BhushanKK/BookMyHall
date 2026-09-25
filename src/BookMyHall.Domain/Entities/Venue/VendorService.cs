using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class VendorService : BaseEntity
{
    public Guid VendorServiceId { get; set; }
    public Guid VendorId { get; set; }
    public Guid VendorSubCategoryId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PricingType { get; set; } = "StartingFrom";
    public decimal? BasePrice { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? UnitName { get; set; }
    public int? MinimumQuantity { get; set; }
    public int? MaximumQuantity { get; set; }
    public bool IsPackage { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public virtual Vendors Vendor { get; set; } = null!;
    public virtual VendorSubCategory VendorSubCategory { get; set; } = null!;
}
