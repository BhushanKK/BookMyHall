using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Identity;

public class RolePermissionDto
{
    [JsonIgnore]
    public Guid RolePermissionId { get; set; }

    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }
}