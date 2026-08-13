using System.Text.Json.Serialization;
using BookMyHall.Domain.Enums;

namespace BookMyHall.Application.Features.Venue;

public class HallDto
{
    [JsonIgnore]
    public Guid HallId { get; set; }
    public Guid HallOwnerId { get; init; }
    public Guid HallCategoryId { get; init; }
    public Guid? CancellationPolicyId { get; init; }
    public string HallName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string AddressLine1 { get; init; } = string.Empty;
    public string? AddressLine2 { get; init; }
    public Guid AreaId { get; init; }
    public string? Pincode { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public string ContactPersonName { get; init; } = string.Empty;
    public string MobileNumber { get; init; } = string.Empty;
    public string? AlternateMobileNumber { get; init; }
    public string? EmailAddress { get; init; }
    public string? Website { get; init; }
    public int? MinimumCapacity { get; init; }
    public int? MaximumCapacity { get; init; }
    public TimeSpan? CheckInTime { get; init; }
    public TimeSpan? CheckOutTime { get; init; }
    public string? GoogleMapLocationUrl { get; init; }

    [JsonIgnore]
    public HallApprovalStatus ApprovalStatus { get; set; }

    [JsonIgnore]
    public HallVerificationStatus VerificationStatus { get; set; }

    [JsonIgnore]
    public bool IsActive { get; set; }
}