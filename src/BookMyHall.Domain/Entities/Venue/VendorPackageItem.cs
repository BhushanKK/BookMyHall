using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class VendorPackageItem : BaseEntity
{
    public Guid VendorPackageItemId { get; set; }
    public Guid VendorPackageId { get; set; }
    public Guid VendorServiceId { get; set; }
    public int Quantity { get; set; } = 1;
    public int DisplayOrder { get; set; }
    public bool IsDeleted { get; set; }
    public virtual VendorPackage VendorPackage { get; set; } = null!;
    public virtual VendorService VendorService { get; set; } = null!;
}
