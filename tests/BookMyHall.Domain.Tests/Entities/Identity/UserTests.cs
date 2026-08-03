using FluentAssertions;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class UserTests
{
    [Fact]
    public void User_Should_Be_Active_By_Default()
    {
        var user = new User();

        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void User_Should_Assign_UserId()
    {
        var user = new User();
        var id = Guid.NewGuid();

        user.UserId = id;

        user.UserId.Should().Be(id);
    }

    [Fact]
    public void User_Should_Assign_FirstName()
    {
        var user = new User();

        user.FirstName = "Rock";

        user.FirstName.Should().Be("Rock");
    }

    [Fact]
    public void User_Should_Assign_MiddleName()
    {
        var user = new User();

        user.MiddleName = "Michael";

        user.MiddleName.Should().Be("Michael");
    }

    [Fact]
    public void User_Should_Assign_LastName()
    {
        var user = new User();

        user.LastName = "Doe";

        user.LastName.Should().Be("Doe");
    }

    [Fact]
    public void User_Should_Assign_MobileNumber()
    {
        var user = new User();

        user.MobileNumber = "9876543210";

        user.MobileNumber.Should().Be("9876543210");
    }

    [Fact]
    public void User_Should_Assign_EmailAddress()
    {
        var user = new User();

        user.EmailAddress = "john@example.com";

        user.EmailAddress.Should().Be("john@example.com");
    }

    [Fact]
    public void User_Should_Assign_PasswordHash()
    {
        var user = new User();

        user.PasswordHash = "hashedPassword";

        user.PasswordHash.Should().Be("hashedPassword");
    }

    [Fact]
    public void User_Should_Assign_ProfileImageUrl()
    {
        var user = new User();

        user.ProfileImageUrl = "https://example.com/profile.jpg";

        user.ProfileImageUrl.Should().Be("https://example.com/profile.jpg");
    }

    [Fact]
    public void User_Should_Assign_IsMobileVerified()
    {
        var user = new User();

        user.IsMobileVerified = true;

        user.IsMobileVerified.Should().BeTrue();
    }

    [Fact]
    public void User_Should_Assign_IsEmailVerified()
    {
        var user = new User();

        user.IsEmailVerified = true;

        user.IsEmailVerified.Should().BeTrue();
    }

    [Fact]
    public void User_Should_Assign_IsActive()
    {
        var user = new User();

        user.IsActive = false;

        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void User_Should_Assign_All_Properties()
    {
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
            ProfileImageUrl = "https://example.com/profile.jpg",
            IsMobileVerified = true,
            IsEmailVerified = true,
            IsActive = false
        };

        user.UserId.Should().Be(userId);
        user.FirstName.Should().Be("Rock");
        user.MiddleName.Should().Be("Michael");
        user.LastName.Should().Be("Doe");
        user.MobileNumber.Should().Be("9876543210");
        user.EmailAddress.Should().Be("john@example.com");
        user.PasswordHash.Should().Be("hashedPassword");
        user.ProfileImageUrl.Should().Be("https://example.com/profile.jpg");
        user.IsMobileVerified.Should().BeTrue();
        user.IsEmailVerified.Should().BeTrue();
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void User_Should_Have_Default_Values()
    {
        var user = new User();
        user.UserId.Should().Be(Guid.Empty);
        user.FirstName.Should().BeEmpty();
        user.MiddleName.Should().BeNull();
        user.LastName.Should().BeNull();
        user.MobileNumber.Should().BeEmpty();
        user.EmailAddress.Should().BeEmpty();
        user.PasswordHash.Should().BeEmpty(); // Changed
        user.ProfileImageUrl.Should().BeNull();
        user.IsMobileVerified.Should().BeFalse();
        user.IsEmailVerified.Should().BeFalse();
        user.IsActive.Should().BeTrue();
    }
}