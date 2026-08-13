using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Venue;
public class HallImage : BaseEntity
{
    public Guid HallImageId { get; set; }
    public Guid HallId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string DisplayOrder { get; set; } = string.Empty;
    public bool IsCoverImage { get; set; }
    public bool IsActive { get; set; }
}