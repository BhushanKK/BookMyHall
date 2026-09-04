using BookMyHall.Domain.Enums;

namespace BookMyHall.Domain.Dtos;
public sealed class HallListDto
{
    public Guid HallId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string HallOwnerName { get; set; } = string.Empty;
    public string HallCategoryName { get; set; } = string.Empty;
    public string? CancellationPolicyName { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string? CityName { get; set; }
    public string? DistrictName { get; set; }
    public string? StateName { get; set; }
    public string? CountryName { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Pincode { get; set; }
    public int? MinimumCapacity { get; set; }
    public int? MaximumCapacity { get; set; }
    public bool IsActive { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? MobileNumber { get; set; }
    public string ApprovalStatus { get; set; } = HallApprovalStatus.Pending.ToString();
    public string VerificationStatus { get; set; } = HallVerificationStatus.Pending.ToString();
}