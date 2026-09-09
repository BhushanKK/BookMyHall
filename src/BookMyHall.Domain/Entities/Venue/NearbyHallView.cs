namespace BookMyHall.Domain.Venue;

public sealed class NearbyHallView
{
    public Guid HallId { get; set; }
    public Guid HallOwnerId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? HallOwnerName { get; set; }
    public string? HallCategoryName { get; set; }
    public string? CancellationPolicyName { get; set; }
    public Guid? AreaId { get; set; }
    public string? AreaName { get; set; }
    public Guid? CityId { get; set; }
    public string? CityName { get; set; }
    public Guid? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public Guid? StateId { get; set; }
    public string? StateName { get; set; }
    public Guid? CountryId { get; set; }
    public string? CountryName { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Pincode { get; set; }
    public int? MinimumCapacity { get; set; }
    public int? MaximumCapacity { get; set; }
    public bool IsActive { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? MobileNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? CheckInTime { get; set; }
    public string? CheckOutTime { get; set; }
    public string? ApprovalStatus { get; set; }
    public string? VerificationStatus { get; set; }
    public double DistanceKm { get; set; }
}