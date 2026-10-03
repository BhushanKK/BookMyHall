namespace BookMyHall.Contracts.Venue;

public sealed class VendorImageDto
{
    public Guid VendorImageId { get; set; }
    public Guid VendorId { get; set; }
    public Guid? VendorServiceId { get; set; }
    public Guid? VendorCategoryId { get; set; }
    public Guid? VendorSubCategoryId { get; set; }
    public string? ServiceName { get; set; }
    public string? VendorCategoryName { get; set; }
    public string? VendorSubCategoryName { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsCoverImage { get; set; }
    public bool IsActive { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
}
