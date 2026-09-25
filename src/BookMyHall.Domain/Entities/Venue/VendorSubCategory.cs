using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class VendorSubCategory : BaseEntity
{
    public Guid VendorSubCategoryId { get; set; }
    public Guid VendorCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public virtual VendorCategory VendorCategory { get; set; } = null!;
}
