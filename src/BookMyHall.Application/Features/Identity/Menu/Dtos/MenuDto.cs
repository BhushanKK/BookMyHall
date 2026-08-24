using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Identity;

public class MenuDto
{
    [JsonIgnore]
    public Guid MenuId { get; set; }
    public Guid? ParentMenuId { get; init; }
    public string MenuName { get; init; } = string.Empty;
    public string MenuCode { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string? Route { get; init; }
    public int DisplayOrder { get; init; }
    public short Level { get; init; } = 1;
    public bool IsMenu { get; init; } = true;
    public bool IsActive { get; init; } = true;
}