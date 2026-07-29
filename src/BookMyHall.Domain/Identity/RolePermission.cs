using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class RolePermission : BaseEntity
{
    public Guid RolePermissionId { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}