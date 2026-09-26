using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class VendorPackage : BaseEntity
{
    public Guid VendorPackageId { get; set; }
    public Guid VendorId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int? DurationMinutes { get; set; }
    public int? GuestLimit { get; set; }
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public virtual Vendors Vendor { get; set; } = null!;
}
