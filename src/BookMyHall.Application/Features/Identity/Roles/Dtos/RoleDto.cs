using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Identity;

public class RoleDto
{
    [JsonIgnore]
    public Guid RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}