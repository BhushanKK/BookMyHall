namespace BookMyHall.Domain.Dtos;
public sealed class HallListDto
{
    public Guid HallId { get; init; }
    public string HallName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string HallOwnerName { get; init; } = string.Empty;
    public string HallCategoryName { get; init; } = string.Empty;
    public string? CancellationPolicyName { get; init; }
    public string AreaName { get; init; } = string.Empty;
    public string? CityName { get; init; }
    public string? DistrictName { get; init; }
    public string? StateName { get; init; }
    public string? CountryName { get; init; }
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? Pincode { get; init; }
    public int? MinimumCapacity { get; init; }
    public int? MaximumCapacity { get; init; }
    public bool IsActive { get; init; }
}