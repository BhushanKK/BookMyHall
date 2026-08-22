using System.Text.Json.Serialization;

using BookMyHall.Domain.Common;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Domain.Entities.Identity;

public sealed class Role : BaseEntity
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    [JsonIgnore]
    public ICollection<UserRole> UserRoles { get; set; } = [];
    [JsonIgnore]
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public void Deactivate()
    {
        IsActive = false;
    }
}