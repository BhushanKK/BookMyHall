using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Entities.Identity;

public class Role : BaseEntity
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}