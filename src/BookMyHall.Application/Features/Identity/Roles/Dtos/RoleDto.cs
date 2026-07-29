using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Identity;

public class RoleDto
{
    [JsonIgnore]
    public Guid RoleId { get; set; }
    public string RoleName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}