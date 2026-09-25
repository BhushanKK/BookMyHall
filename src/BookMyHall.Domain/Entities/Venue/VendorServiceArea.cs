using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class VendorServiceArea : BaseEntity
{
    public Guid VendorServiceAreaId { get; set; }
    public Guid VendorId { get; set; }
    public Guid? StateId { get; set; }
    public Guid? CityId { get; set; }
    public Guid? AreaId { get; set; }
    public decimal? ServiceRadiusKm { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public virtual Vendors Vendor { get; set; } = null!;
}
