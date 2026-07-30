using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Domain.Identity;
public sealed class UserRole
{
    public Guid UserRoleId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}