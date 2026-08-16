using FluentAssertions;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class PermissionTests
{
    [Fact]
    public void Permission_Should_Assign_PermissionId()
    {
        var permission = new Permission();
        var id = Guid.NewGuid();

        permission.PermissionId = id;

        permission.PermissionId.Should().Be(id);
    }

    [Fact]
    public void Permission_Should_Assign_PermissionName()
    {
        var permission = new Permission();

        permission.PermissionName = "User.Create";

        permission.PermissionName.Should().Be("User.Create");
    }

    [Fact]
    public void Permission_Should_Assign_Description()
    {
        var permission = new Permission();

        permission.Description = "Allows creating users.";

        permission.Description.Should().Be("Allows creating users.");
    }

    [Fact]
    public void Permission_Should_Assign_IsActive()
    {
        var permission = new Permission();

        permission.IsActive = true;

        permission.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Permission_Should_Assign_All_Properties()
    {
        var permissionId = Guid.NewGuid();

        var permission = new Permission
        {
            PermissionId = permissionId,
            PermissionName = "User.Create",
            Description = "Allows creating users.",
            IsActive = true
        };

        permission.PermissionId.Should().Be(permissionId);
        permission.PermissionName.Should().Be("User.Create");
        permission.Description.Should().Be("Allows creating users.");
        permission.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Permission_Should_Have_Default_Values()
    {
        var permission = new Permission();

        permission.PermissionId.Should().Be(Guid.Empty);
        permission.PermissionName.Should().BeEmpty();
        permission.Description.Should().BeEmpty();
        permission.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_ShouldSet_IsActive_ToFalse()
    {
        // Arrange
        var permission = new Permission
        {
            PermissionId = Guid.NewGuid(),
            PermissionName = "Create Hall",
            Description = "Allows user to create a hall",
            IsActive = true
        };

        // Act
        permission.Deactivate();

        // Assert
        permission.IsActive.Should().BeFalse();
    }
}