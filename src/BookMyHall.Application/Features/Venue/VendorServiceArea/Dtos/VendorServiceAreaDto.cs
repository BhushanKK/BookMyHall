namespace BookMyHall.Application.Features.Venue;
public class VendorServiceAreaDto
{
    public Guid VendorServiceAreaId { get; set; }
    public Guid VendorId { get; set; }
    public Guid? StateId { get; set; }
    public Guid? CityId { get; set; }
    public Guid? AreaId { get; set; }
    public decimal? ServiceRadiusKm { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }
}