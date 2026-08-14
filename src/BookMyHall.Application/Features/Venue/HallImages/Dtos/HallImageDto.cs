namespace BookMyHall.Contracts.Venue;

public sealed class HallImageDto
{
    public Guid HallImageId { get; set; }
    public Guid HallId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsCoverImage { get; set; }
    public bool IsActive { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
}