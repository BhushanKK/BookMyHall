using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Entities.Identity;

public sealed class Menu : BaseEntity
{
    public Guid MenuId { get; set; }

    public Guid? ParentMenuId { get; set; }

    public string MenuName { get; set; } = string.Empty;

    public string MenuCode { get; set; } = string.Empty;

    public string? Icon { get; set; }

    public string? Route { get; set; }

    public int DisplayOrder { get; set; }

    public short Level { get; set; } = 1;

    public bool IsMenu { get; set; } = true;

    public bool IsActive { get; set; } = true;

    public Menu? ParentMenu { get; set; }

    public ICollection<Menu> ChildMenus { get; set; } = [];
    public ICollection<MenuRolePermission> MenuRolePermissions { get; set; } = [];
}