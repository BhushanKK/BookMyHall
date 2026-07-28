using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Entities.Identity;
public class Role : BaseEntity
{
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}