using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class UserRole : BaseEntity
{
    public Guid UserRoleId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}