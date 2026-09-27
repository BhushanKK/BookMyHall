using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Venue;
public class VendorSubCategoryDto
{
    [JsonIgnore]
    public Guid VendorSubCategoryId { get; set; }
    public Guid VendorCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}