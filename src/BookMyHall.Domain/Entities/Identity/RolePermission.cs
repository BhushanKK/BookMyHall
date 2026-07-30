using BookMyHall.Domain.Common;
using BookMyHall.Domain.Entities.Identity;
namespace BookMyHall.Domain.Identity;
public class RolePermission : BaseEntity
{
    public Guid RolePermissionId { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}