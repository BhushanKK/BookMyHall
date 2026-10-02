using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Venue;

public class VendorCategoryDto
{
    [JsonIgnore]
    public Guid VendorCategoryId { get; set; }
    public string VendorCategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}