using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Entities.Identity;

public class User : BaseEntity
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string? EmailAddress { get; set; }
    public string? PasswordHash { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsMobileVerified { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; } = true;
}