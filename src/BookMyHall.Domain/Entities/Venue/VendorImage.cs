using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class VendorImage : BaseEntity
{
    public Guid VendorImageId { get; set; }
    public Guid VendorId { get; set; }
    public Guid? VendorServiceId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int DisplayOrder { get; set; } = 1;
    public bool IsCoverImage { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual Vendor Vendor { get; set; } = null!;
    public virtual VendorService? VendorService { get; set; }

    public void UpdateMetadata(int displayOrder, bool isCoverImage, bool isActive)
    {
        DisplayOrder = displayOrder;
        IsCoverImage = isCoverImage;
        IsActive = isActive;
    }

    public void ReplaceImage(string imageUrl)
    {
        ImageUrl = imageUrl;
        ThumbnailUrl = null;
    }

    public void SetThumbnailUrl(string thumbnailUrl) => ThumbnailUrl = thumbnailUrl;
}
