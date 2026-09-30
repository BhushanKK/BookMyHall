namespace BookMyHall.Application.Features.Venue;

public class VendorPackageItemDto
{
    public Guid VendorPackageItemId { get; set; }
    public Guid VendorPackageId { get; set; }
    public Guid VendorServiceId { get; set; }
    public int Quantity { get; set; }
    public int DisplayOrder { get; set; }
}