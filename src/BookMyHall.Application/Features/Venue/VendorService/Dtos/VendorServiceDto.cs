namespace BookMyHall.Application.Features.Venue;
public class VendorServiceDto
{
    public Guid VendorServiceId { get; set; }
    public Guid VendorId { get; set; }
    public Guid UserId { get; set; }
    public Guid VendorCategoryId { get; set; }
    public Guid VendorSubCategoryId { get; set; }
    public string? BusinessName { get; set; }
    public string? VendorCategoryName { get; set; }
    public string? VendorSubCategoryName { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PricingType { get; set; } = "StartingFrom";
    public decimal? BasePrice { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? UnitName { get; set; }
    public int? MinimumQuantity { get; set; }
    public int? MaximumQuantity { get; set; }
    public bool IsPackage { get; set; }
    public bool IsActive { get; set; }
}
