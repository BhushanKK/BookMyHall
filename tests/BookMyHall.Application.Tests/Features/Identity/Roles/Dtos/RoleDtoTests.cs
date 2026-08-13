using System.Text.Json;
using BookMyHall.Application.Features.Identity;
using FluentAssertions;

namespace BookMyHall.Application.Tests.Features.Identity.Role.Dtos;

public sealed class RoleDtoTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Act
        var dto = new RoleDto();

        // Assert
        dto.RoleId.Should().Be(Guid.Empty);
        dto.RoleName.Should().BeEmpty();
        dto.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Properties_ShouldSetAndGetValues()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        // Act
        var dto = new RoleDto
        {
            RoleId = roleId,
            RoleName = "Administrator",
            IsActive = true
        };

        // Assert
        dto.RoleId.Should().Be(roleId);
        dto.RoleName.Should().Be("Administrator");
        dto.IsActive.Should().BeTrue();
    }

    [Fact]
    public void RoleId_ShouldBeSerializable_WhenJsonIgnoreIsApplied()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        var dto = new RoleDto
        {
            RoleId = roleId,
            RoleName = "Administrator",
            IsActive = true
        };

        // Act
        var json = JsonSerializer.Serialize(dto);

        // Assert
        json.Should().NotContain(roleId.ToString());

        json.Should().Contain("RoleName");
        json.Should().Contain("IsActive");
    }

    [Fact]
    public void JsonSerialization_ShouldIgnoreRoleId()
    {
        // Arrange
        var dto = new RoleDto
        {
            RoleId = Guid.NewGuid(),
            RoleName = "Administrator",
            IsActive = true
        };

        // Act
        var json = JsonSerializer.Serialize(dto);

        // Assert
        using var document = JsonDocument.Parse(json);

        document.RootElement.TryGetProperty("RoleId", out _)
            .Should().BeFalse();

        document.RootElement.TryGetProperty("RoleName", out _)
            .Should().BeTrue();

        document.RootElement.TryGetProperty("IsActive", out _)
            .Should().BeTrue();
    }

    [Fact]
    public void Should_AllowInactiveRole()
    {
        // Act
        var dto = new RoleDto
        {
            RoleName = "Guest",
            IsActive = false
        };

        // Assert
        dto.RoleName.Should().Be("Guest");
        dto.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Should_AllowEmptyRoleName()
    {
        // Act
        var dto = new RoleDto
        {
            RoleName = string.Empty,
            IsActive = true
        };

        // Assert
        dto.RoleName.Should().BeEmpty();
        dto.IsActive.Should().BeTrue();
    }
}