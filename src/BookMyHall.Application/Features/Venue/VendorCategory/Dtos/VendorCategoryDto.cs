namespace BookMyHall.Application.Features.Venue;

public class VendorCategoryDto
{
    public Guid VendorCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}