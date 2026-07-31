using BookMyHall.Domain.Common;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Domain.Entities.Identity;

public class User : BaseEntity
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string? EmailAddress { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public bool IsMobileVerified { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<UserRole> UserRoles { get; set; } = [];

    public void VerifyMobile()
    {
        IsMobileVerified = true;
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public string FullName =>
        string.Join(" ",
            new[]
            {
                FirstName,
                MiddleName,
                LastName
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
}