using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Venue;

public class VendorPackageDto
{
    [JsonIgnore]
    public Guid VendorPackageId { get; set; }
    public Guid VendorId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int? DurationMinutes { get; set; }
    public int? GuestLimit { get; set; }
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
