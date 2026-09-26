using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class VendorCategory : BaseEntity
{
    public Guid VendorCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}
