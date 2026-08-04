using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Master;
public class ServiceDto
{
    [JsonIgnore]
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceIcon { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}