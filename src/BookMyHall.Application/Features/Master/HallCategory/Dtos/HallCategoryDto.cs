using System.Text.Json.Serialization;

namespace BookMyHall.Domain.Masters;   
public class HallCategoryDto
{
    [JsonIgnore]
    public Guid HallCategoryId { get; set; }
    public string HallCategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; } 
}