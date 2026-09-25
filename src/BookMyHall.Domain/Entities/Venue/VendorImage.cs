using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class VendorImage : BaseEntity
{
    public Guid VendorImageId { get; set; }
    public Guid VendorId { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string? OriginalFileName { get; set; }
    public string? ContentType { get; set; }
    public long? FileSize { get; set; }
    public string ImageType { get; set; } = "Gallery";
    public int DisplayOrder { get; set; }
    public bool IsCoverImage { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public virtual Vendors Vendor { get; set; } = null!;
}
