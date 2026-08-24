public sealed class MenuRolePermissionDto
{
    public Guid MenuRolePermissionId { get; set; }
    public Guid RoleId { get; set; }
    public Guid MenuId { get; set; }
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanPrint { get; set; }
    public bool CanExport { get; set; }
}