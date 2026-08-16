using BookMyHall.Application.Features.Identity.Authentication;
using FluentAssertions;

namespace BookMyHall.Application.Tests.Features.Identity.Authentication;

public sealed class RefreshTokenWithUserDtoTests
{
    [Fact]
    public void Should_Create_Dto_With_Default_Values()
    {
        // Act
        var dto = new RefreshTokenWithUserDto();

        // Assert
        dto.RefreshTokenId.Should().Be(Guid.Empty);
        dto.Token.Should().BeNull();
        dto.ExpiresAt.Should().Be(default);
        dto.IsRevoked.Should().BeFalse();
        dto.RevokedBy.Should().Be(Guid.Empty);
        dto.RevokedAt.Should().Be(default);
        dto.UserId.Should().Be(Guid.Empty);
        dto.FullName.Should().BeNull();
        dto.MobileNumber.Should().BeNull();
        dto.EmailAddress.Should().BeNull();
        dto.TokenVersion.Should().Be(0);
        dto.IsActive.Should().BeFalse();

        dto.Roles.Should().NotBeNull();
        dto.Roles.Should().BeEmpty();
    }

    [Fact]
    public void Should_Set_All_Properties_Correctly()
    {
        // Arrange
        var refreshTokenId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var revokedBy = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);
        var revokedAt = DateTimeOffset.UtcNow;

        var roles = new List<string>
        {
            "Admin",
            "User"
        };

        // Act
        var dto = new RefreshTokenWithUserDto
        {
            RefreshTokenId = refreshTokenId,
            Token = "refresh-token-value",
            ExpiresAt = expiresAt,
            IsRevoked = true,
            RevokedBy = revokedBy,
            RevokedAt = revokedAt,
            UserId = userId,
            FullName = "Bhushan Kachave",
            MobileNumber = "9876543210",
            EmailAddress = "bhushan@example.com",
            TokenVersion = 2,
            IsActive = true,
            Roles = roles
        };

        // Assert
        dto.RefreshTokenId.Should().Be(refreshTokenId);
        dto.Token.Should().Be("refresh-token-value");
        dto.ExpiresAt.Should().Be(expiresAt);
        dto.IsRevoked.Should().BeTrue();
        dto.RevokedBy.Should().Be(revokedBy);
        dto.RevokedAt.Should().Be(revokedAt);
        dto.UserId.Should().Be(userId);
        dto.FullName.Should().Be("Bhushan Kachave");
        dto.MobileNumber.Should().Be("9876543210");
        dto.EmailAddress.Should().Be("bhushan@example.com");
        dto.TokenVersion.Should().Be(2);
        dto.IsActive.Should().BeTrue();

        dto.Roles.Should().NotBeNull();
        dto.Roles.Should().HaveCount(2);
        dto.Roles.Should().ContainInOrder("Admin", "User");
    }

    [Fact]
    public void Should_Allow_Multiple_Roles()
    {
        // Arrange
        var dto = new RefreshTokenWithUserDto
        {
            Roles =
            [
                "Admin",
                "Manager",
                "User"
            ]
        };

        // Assert
        dto.Roles.Should().HaveCount(3);
        dto.Roles.Should().Contain("Admin");
        dto.Roles.Should().Contain("Manager");
        dto.Roles.Should().Contain("User");
    }

    [Fact]
    public void Should_Allow_Empty_Roles()
    {
        // Arrange
        var dto = new RefreshTokenWithUserDto
        {
            Roles = []
        };

        // Assert
        dto.Roles.Should().NotBeNull();
        dto.Roles.Should().BeEmpty();
    }

    [Fact]
    public void Should_Update_Revocation_Properties()
    {
        // Arrange
        var dto = new RefreshTokenWithUserDto();

        var revokedBy = Guid.NewGuid();
        var revokedAt = DateTimeOffset.UtcNow;

        // Act
        dto.IsRevoked = true;
        dto.RevokedBy = revokedBy;
        dto.RevokedAt = revokedAt;

        // Assert
        dto.IsRevoked.Should().BeTrue();
        dto.RevokedBy.Should().Be(revokedBy);
        dto.RevokedAt.Should().Be(revokedAt);
    }

    [Fact]
    public void Should_Update_TokenVersion()
    {
        // Arrange
        var dto = new RefreshTokenWithUserDto();

        // Act
        dto.TokenVersion = 5;

        // Assert
        dto.TokenVersion.Should().Be(5);
    }
}