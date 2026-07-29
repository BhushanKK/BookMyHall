using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class Permission : BaseEntity
{
    public Guid PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}