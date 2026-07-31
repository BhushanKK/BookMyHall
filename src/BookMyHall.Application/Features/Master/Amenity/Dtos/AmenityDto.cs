using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Master;

public class AmenityDto
{
    [JsonIgnore]
    public Guid AmenityId { get; set; }
    public string AmenityName { get; set; } = string.Empty;
    public string AmenityIcon { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}