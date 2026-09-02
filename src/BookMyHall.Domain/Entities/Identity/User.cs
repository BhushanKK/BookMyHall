using BookMyHall.Domain.Common;
using BookMyHall.Domain.Enums;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Domain.Entities.Identity;

public class User : BaseEntity
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? MobileNumber { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public bool IsMobileVerified { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Increment whenever the password changes or all sessions are revoked.
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
    public ICollection<PasswordResetToken> PasswordResetTokens { get; private set; } = [];
    public ICollection<EmailVerificationToken> EmailVerificationTokens { get; private set; } = [];

    public void UpdateUserProfile(
        string firstName,
        string? middleName,
        string? lastName,
        string mobileNumber,
        DateTimeOffset? dateOfBirth,
        Gender? gender,
        string emailAddress)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        MobileNumber = mobileNumber;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        EmailAddress = emailAddress;
    }

    public void UpdateProfilePicture(string? profileImageUrl) 
        => ProfileImageUrl = profileImageUrl;

    public string FullName => string.Join
    (
        " ",
        new[]
        {
            FirstName,
            MiddleName,
            LastName
        }.Where(x => !string.IsNullOrWhiteSpace(x))
    );

    public void VerifyMobile() => IsMobileVerified = true;

    public void VerifyEmail() => IsEmailVerified = true;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void UpdatePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        PasswordHash = passwordHash;
        PasswordChangedAt = DateTimeOffset.UtcNow;
        InvalidateTokens();
    }

    public void RevokeAllSessions() => InvalidateTokens();
    private void InvalidateTokens() => TokenVersion++;
}
