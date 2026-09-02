using BookMyHall.Domain.Dtos;
using FluentAssertions;

namespace BookMyHall.Application.Tests.Features.Identity.Authentication;

public sealed class UserLoginDtoTests
{
    [Fact]
    public void Should_Create_Dto_With_Default_Values()
    {
        // Act
        var dto = new UserLoginDto();

        // Assert
        dto.UserId.Should().Be(Guid.Empty);
        dto.MobileNumber.Should().BeNullOrEmpty();
        dto.EmailAddress.Should().BeNullOrEmpty();
        dto.FullName.Should().BeNullOrEmpty();
        dto.PasswordHash.Should().BeNullOrEmpty();
        dto.TokenVersion.Should().Be(0);

        dto.Roles.Should().NotBeNull();
        dto.Roles.Should().BeEmpty();
    }

    [Fact]
    public void Should_Set_All_Properties_Correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var adminRoleId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        var dto = new UserLoginDto
        {
            UserId = userId,
            MobileNumber = "9876543210",
            EmailAddress = "user@example.com",
            FullName = "Bhushan Kachave",
            PasswordHash = "hashed-password-value",
            TokenVersion = 2,
            Roles =
            [
                new JwtRole
                {
                    RoleId = adminRoleId,
                    RoleName = "Admin"
                },
                new JwtRole
                {
                    RoleId = userRoleId,
                    RoleName = "User"
                }
            ]
        };

        // Assert
        dto.UserId.Should().Be(userId);
        dto.MobileNumber.Should().Be("9876543210");
        dto.EmailAddress.Should().Be("user@example.com");
        dto.FullName.Should().Be("Bhushan Kachave");
        dto.PasswordHash.Should().Be("hashed-password-value");
        dto.TokenVersion.Should().Be(2);

        dto.Roles.Should().NotBeNull();
        dto.Roles.Should().HaveCount(2);

        dto.Roles[0].RoleId.Should().Be(adminRoleId);
        dto.Roles[0].RoleName.Should().Be("Admin");

        dto.Roles[1].RoleId.Should().Be(userRoleId);
        dto.Roles[1].RoleName.Should().Be("User");
    }

    [Fact]
    public void Should_Allow_Empty_Roles()
    {
        // Arrange
        var dto = new UserLoginDto
        {
            UserId = Guid.NewGuid(),
            MobileNumber = "9876543210",
            EmailAddress = "user@example.com",
            FullName = "Test User",
            PasswordHash = "hashed-password",
            TokenVersion = 1,
            Roles = []
        };

        // Assert
        dto.Roles.Should().NotBeNull();
        dto.Roles.Should().BeEmpty();
    }
}