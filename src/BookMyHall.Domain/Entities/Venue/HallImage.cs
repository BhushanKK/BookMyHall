using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class HallImage : BaseEntity
{
    public Guid HallImageId { get; set; }
    public Guid HallId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsCoverImage { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation property
    public Hall Hall { get; private set; } = null!;

    private HallImage()
    {
    }

    public HallImage(
    Guid hallImageId,
    Guid hallId,
    string imageUrl,
    string? thumbnailUrl,
    int displayOrder,
    bool isCoverImage,
    Guid? createdBy)
    {
        HallImageId = hallImageId;
        HallId = hallId;
        ImageUrl = imageUrl;
        ThumbnailUrl = thumbnailUrl;
        DisplayOrder = displayOrder;
        IsCoverImage = isCoverImage;
        IsActive = true;
        CreatedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
    }

    public void Update(
    string imageUrl,
    string? thumbnailUrl,
    int displayOrder,
    bool isCoverImage)
    {
        ImageUrl = imageUrl;
        ThumbnailUrl = thumbnailUrl;
        DisplayOrder = displayOrder;
        IsCoverImage = isCoverImage;
    }

    public void SetCoverImage(bool isCoverImage) => IsCoverImage = isCoverImage;
    public void SetActive(bool isActive) => IsActive = isActive;
    public void UpdateDisplayOrder(int displayOrder) => DisplayOrder = displayOrder;
}