using BookMyHall.Domain.Common;
using BookMyHall.Domain.Enums;

namespace BookMyHall.Domain.Venue;

public class Hall : BaseEntity
{
    public Guid HallId { get; set; }
    public Guid HallOwnerId { get; set; }
    public Guid HallCategoryId { get; set; }
    public Guid? CancellationPolicyId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public Guid AreaId { get; set; }
    public string? Pincode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string? AlternateMobileNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? Website { get; set; }
    public int? MinimumCapacity { get; set; }
    public int? MaximumCapacity { get; set; }
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public string? GoogleMapLocationUrl { get; set; }
    public HallApprovalStatus ApprovalStatus { get; set; }
        = HallApprovalStatus.Pending;
    public HallVerificationStatus VerificationStatus { get; set; }
        = HallVerificationStatus.Pending;
    public bool IsActive { get; set; } = true;

    public ICollection<HallImage> HallImages { get; private set; }
    = new List<HallImage>();
}