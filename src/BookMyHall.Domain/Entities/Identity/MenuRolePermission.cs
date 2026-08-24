namespace BookMyHall.Domain.Entities.Identity;

public sealed class MenuRolePermission
{
    public Guid MenuRolePermissionId { get; set; }
    public Guid MenuId { get; set; }
    public Guid RoleId { get; set; }
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanPrint { get; set; }
    public bool CanExport { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public Menu Menu { get; set; } = null!;
    public Role Role { get; set; } = null!;
}