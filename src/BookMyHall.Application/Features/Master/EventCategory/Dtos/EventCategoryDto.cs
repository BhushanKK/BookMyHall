using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Master;

public class EventCategoryDto
{
     [JsonIgnore]
    public Guid EventCategoryId { get; set; }
    public string EventCategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}