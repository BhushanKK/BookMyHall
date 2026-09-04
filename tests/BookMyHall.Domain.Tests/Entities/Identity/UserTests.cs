using FluentAssertions;

using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class UserTests
{
    [Fact]
    public void User_Should_Be_Active_By_Default()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void User_Should_Have_Empty_UserId_By_Default()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.UserId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void User_Should_Assign_UserId()
    {
        // Arrange
        var user = new User();
        var userId = Guid.NewGuid();

        // Act
        user.UserId = userId;

        // Assert
        user.UserId.Should().Be(userId);
    }

    [Fact]
    public void User_Should_Assign_FirstName()
    {
        // Arrange
        var user = new User();

        // Act
        user.FirstName = "Rock";

        // Assert
        user.FirstName.Should().Be("Rock");
    }

    [Fact]
    public void User_Should_Assign_MiddleName()
    {
        // Arrange
        var user = new User();

        // Act
        user.MiddleName = "Michael";

        // Assert
        user.MiddleName.Should().Be("Michael");
    }

    [Fact]
    public void User_Should_Assign_LastName()
    {
        // Arrange
        var user = new User();

        // Act
        user.LastName = "Doe";

        // Assert
        user.LastName.Should().Be("Doe");
    }

    [Fact]
    public void User_Should_Assign_MobileNumber()
    {
        // Arrange
        var user = new User();

        // Act
        user.MobileNumber = "9876543210";

        // Assert
        user.MobileNumber.Should().Be("9876543210");
    }

    [Fact]
    public void User_Should_Assign_EmailAddress()
    {
        // Arrange
        var user = new User();

        // Act
        user.EmailAddress = "john@example.com";

        // Assert
        user.EmailAddress.Should().Be("john@example.com");
    }

    [Fact]
    public void User_Should_Assign_PasswordHash()
    {
        // Arrange
        var user = new User();

        // Act
        user.PasswordHash = "hashedPassword";

        // Assert
        user.PasswordHash.Should().Be("hashedPassword");
    }

    [Fact]
    public void User_Should_Assign_ProfileImageUrl()
    {
        // Arrange
        var user = new User();

        const string profileImageUrl =
            "https://images.bookmyhall.com/users/profile.jpg";

        // Act
        user.ProfileImageUrl = profileImageUrl;

        // Assert
        user.ProfileImageUrl.Should().Be(profileImageUrl);
    }

    [Fact]
    public void User_Should_Have_Null_ProfileImageUrl_By_Default()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.ProfileImageUrl = string.Empty;
    }

    [Fact]
    public void User_Should_Assign_IsMobileVerified()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsMobileVerified = true;

        // Assert
        user.IsMobileVerified.Should().BeTrue();
    }

    [Fact]
    public void User_Should_Assign_IsEmailVerified()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsEmailVerified = true;

        // Assert
        user.IsEmailVerified.Should().BeTrue();
    }

    [Fact]
    public void User_Should_Assign_IsActive()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = false;

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void User_Should_Have_Default_Values()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.UserId.Should().Be(Guid.Empty);

        user.FirstName.Should().BeEmpty();
        user.MiddleName.Should().BeNull();
        user.LastName.Should().BeNull();

        user.MobileNumber.Should().BeNull();
        user.EmailAddress.Should().BeNullOrEmpty();
        user.PasswordHash.Should().BeNull();

        // Profile image is optional.
        user.ProfileImageUrl.Should().BeNull();

        user.DateOfBirth.Should().BeNull();
        user.Gender.Should().BeNull();

        user.IsMobileVerified.Should().BeFalse();
        user.IsEmailVerified.Should().BeFalse();
        user.IsActive.Should().BeTrue();
        user.IsDeleted.Should().BeFalse();

        user.TokenVersion.Should().Be(1);

        user.LastLoginAt.Should().BeNull();
        user.PasswordChangedAt.Should().BeNull();

        user.UserRoles.Should().NotBeNull();
        user.UserRoles.Should().BeEmpty();

        user.PasswordResetTokens.Should().NotBeNull();
        user.PasswordResetTokens.Should().BeEmpty();

        user.EmailVerificationTokens.Should().NotBeNull();
        user.EmailVerificationTokens.Should().BeEmpty();
    }

    [Fact]
    public void User_Should_Assign_All_Properties()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var user = new User
        {
            UserId = userId,
            FirstName = "Rock",
            MiddleName = "Michael",
            LastName = "Doe",
            MobileNumber = "9876543210",
            EmailAddress = "john@example.com",
            PasswordHash = "hashedPassword",
            ProfileImageUrl = "https://images.bookmyhall.com/users/profile.jpg",
            IsMobileVerified = true,
            IsEmailVerified = true,
            IsActive = false
        };

        // Assert
        user.UserId.Should().Be(userId);
        user.FirstName.Should().Be("Rock");
        user.MiddleName.Should().Be("Michael");
        user.LastName.Should().Be("Doe");
        user.MobileNumber.Should().Be("9876543210");
        user.EmailAddress.Should().Be("john@example.com");
        user.PasswordHash.Should().Be("hashedPassword");

        user.ProfileImageUrl.Should()
            .Be("https://images.bookmyhall.com/users/profile.jpg");

        user.IsMobileVerified.Should().BeTrue();
        user.IsEmailVerified.Should().BeTrue();
        user.IsActive.Should().BeFalse();
    }

    #region FullName

    [Fact]
    public void FullName_Should_Return_FirstName_Only_When_MiddleName_And_LastName_Are_Null()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Bhushan",
            MiddleName = null,
            LastName = null
        };

        // Act
        var result = user.FullName;

        // Assert
        result.Should().Be("Bhushan");
    }

    [Fact]
    public void FullName_Should_Return_FirstName_And_LastName_When_MiddleName_Is_Null()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Bhushan",
            MiddleName = null,
            LastName = "Kachave"
        };

        // Act
        var result = user.FullName;

        // Assert
        result.Should().Be("Bhushan Kachave");
    }

    [Fact]
    public void FullName_Should_Return_All_Names_When_All_Names_Are_Present()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Bhushan",
            MiddleName = "Dattatray",
            LastName = "Kachave"
        };

        // Act
        var result = user.FullName;

        // Assert
        result.Should()
            .Be("Bhushan Dattatray Kachave");
    }

    [Fact]
    public void FullName_Should_Ignore_Whitespace_Names()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Bhushan",
            MiddleName = "   ",
            LastName = "Kachave"
        };

        // Act
        var result = user.FullName;

        // Assert
        result.Should().Be("Bhushan Kachave");
    }

    #endregion

    #region Verification

    [Fact]
    public void VerifyMobile_Should_Set_IsMobileVerified_To_True()
    {
        // Arrange
        var user = new User
        {
            IsMobileVerified = false
        };

        // Act
        user.VerifyMobile();

        // Assert
        user.IsMobileVerified.Should().BeTrue();
    }

    [Fact]
    public void VerifyEmail_Should_Set_IsEmailVerified_To_True()
    {
        // Arrange
        var user = new User
        {
            IsEmailVerified = false
        };

        // Act
        user.VerifyEmail();

        // Assert
        user.IsEmailVerified.Should().BeTrue();
    }

    #endregion

    #region Activation

    [Fact]
    public void Activate_Should_Set_IsActive_To_True()
    {
        // Arrange
        var user = new User
        {
            IsActive = false
        };

        // Act
        user.Activate();

        // Assert
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_Should_Set_IsActive_To_False()
    {
        // Arrange
        var user = new User
        {
            IsActive = true
        };

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
    }

    #endregion

    #region Password

    [Fact]
    public void UpdatePassword_Should_Update_PasswordHash()
    {
        // Arrange
        var user = new User
        {
            PasswordHash = "old-password-hash"
        };

        const string newPasswordHash =
            "new-password-hash";

        // Act
        user.UpdatePassword(newPasswordHash);

        // Assert
        user.PasswordHash.Should()
            .Be(newPasswordHash);
    }

    [Fact]
    public void UpdatePassword_Should_Set_PasswordChangedAt()
    {
        // Arrange
        var user = new User
        {
            PasswordHash = "old-password-hash"
        };

        var beforeUpdate = DateTimeOffset.UtcNow;

        // Act
        user.UpdatePassword("new-password-hash");

        var afterUpdate = DateTimeOffset.UtcNow;

        // Assert
        user.PasswordChangedAt.Should().NotBeNull();

        user.PasswordChangedAt.Should()
            .BeOnOrAfter(beforeUpdate);

        user.PasswordChangedAt.Should()
            .BeOnOrBefore(afterUpdate);
    }

    [Fact]
    public void UpdatePassword_Should_Increment_TokenVersion()
    {
        // Arrange
        var user = new User
        {
            TokenVersion = 1
        };

        // Act
        user.UpdatePassword("new-password-hash");

        // Assert
        user.TokenVersion.Should().Be(2);
    }

    [Fact]
    public void UpdatePassword_Should_Invalidate_Existing_Tokens()
    {
        // Arrange
        var user = new User
        {
            TokenVersion = 5
        };

        // Act
        user.UpdatePassword("new-password-hash");

        // Assert
        user.TokenVersion.Should().Be(6);
    }

    [Fact]
    public void UpdatePassword_Should_Increment_TokenVersion_Each_Time()
    {
        // Arrange
        var user = new User
        {
            TokenVersion = 1
        };

        // Act
        user.UpdatePassword("password-hash-1");
        user.UpdatePassword("password-hash-2");

        // Assert
        user.TokenVersion.Should().Be(3);
    }

    #endregion

    #region Sessions

    [Fact]
    public void RevokeAllSessions_Should_Increment_TokenVersion()
    {
        // Arrange
        var user = new User
        {
            TokenVersion = 1
        };

        // Act
        user.RevokeAllSessions();

        // Assert
        user.TokenVersion.Should().Be(2);
    }

    [Fact]
    public void RevokeAllSessions_Should_Invalidate_Existing_Tokens()
    {
        // Arrange
        var user = new User
        {
            TokenVersion = 10
        };

        // Act
        user.RevokeAllSessions();

        // Assert
        user.TokenVersion.Should().Be(11);
    }

    [Fact]
    public void RevokeAllSessions_Should_Increment_TokenVersion_Each_Time()
    {
        // Arrange
        var user = new User
        {
            TokenVersion = 1
        };

        // Act
        user.RevokeAllSessions();
        user.RevokeAllSessions();

        // Assert
        user.TokenVersion.Should().Be(3);
    }

    #endregion
}