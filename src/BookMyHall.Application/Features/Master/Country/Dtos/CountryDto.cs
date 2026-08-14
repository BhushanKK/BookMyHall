using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Master;

public class CountryDto
{
    [JsonIgnore]
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string? PhoneCode { get; set; }
    public string? CurrencyCode { get; set; }
    public bool IsActive { get; set; }
}