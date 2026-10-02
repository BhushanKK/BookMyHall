namespace BookMyHall.Domain.Dtos;

public sealed class VendorListView
{
    public Guid VendorId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? AreaId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string? ContactPersonName { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public string? AlternateMobileNumber { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? YoutubeUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Pincode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public short? EstablishedYear { get; set; }
    public bool IsVerified { get; set; }
    public bool IsActive { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
}