using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Entities.Identity;

public sealed class Role : BaseEntity
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public void Deactivate()
    {
        IsActive = false;
    }
}