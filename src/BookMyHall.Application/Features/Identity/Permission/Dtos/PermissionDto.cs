using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Identity;

public class PermissionDto
{
    [JsonIgnore]
    public Guid PermissionId { get; set; }

    public string PermissionName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}