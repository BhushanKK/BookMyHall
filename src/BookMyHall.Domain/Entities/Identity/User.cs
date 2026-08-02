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

    /// <summary>
    /// Increment whenever user changes password or signs out from all devices.
    /// Existing JWTs become invalid.
    /// </summary>
    public int TokenVersion { get; set; } = 1;

    /// <summary>
    /// Last successful login.
    /// </summary>
    public DateTimeOffset? LastLoginAt { get; set; }

    /// <summary>
    /// Last password change.
    /// </summary>
    public DateTimeOffset? PasswordChangedAt { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public string FullName =>
        string.Join(" ",
            new[]
            {
                FirstName,
                MiddleName,
                LastName
            }.Where(x => !string.IsNullOrWhiteSpace(x)));

    public void VerifyMobile() => IsMobileVerified = true;
    public void VerifyEmail() => IsEmailVerified = true;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void RecordLogin() => LastLoginAt = DateTimeOffset.UtcNow;
    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        PasswordChangedAt = DateTimeOffset.UtcNow;
        InvalidateTokens();
    }
    public void RevokeAllSessions() => InvalidateTokens();
    private void InvalidateTokens() => TokenVersion++;
}