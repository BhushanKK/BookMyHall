using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class Hall : BaseEntity
{
    public Guid HallId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public Guid HallOwnerId { get; set; }
    public Guid HallCategoryId { get; set; }
    public Guid CancellationPolicyId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public Guid AreaId { get; set; } 
    public string Pincode { get; set; } = string.Empty;
    public decimal Latitude { get; set; } = 0;
    public decimal Longitude { get; set; } = 0;
    public string ContactPersonName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string AlternateMobileNumber { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string MinimumCapacity { get; set; } = string.Empty;
    public string MaximumCapacity { get; set; } = string.Empty;
    public TimeSpan CheckInTime { get; set; } = TimeSpan.Zero;
    public TimeSpan CheckOutTime { get; set; } = TimeSpan.Zero;
    public string GoogleMapLocationUrl { get; set; } = string.Empty;
    public string ApprovalStatus { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;

}