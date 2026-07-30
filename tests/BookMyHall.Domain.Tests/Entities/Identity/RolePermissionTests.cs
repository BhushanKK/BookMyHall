using FluentAssertions;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class RolePermissionTests
{
    [Fact]
    public void RolePermission_Should_Assign_RolePermissionId()
    {
        var rolePermission = new RolePermission();
        var id = Guid.NewGuid();

        rolePermission.RolePermissionId = id;

        rolePermission.RolePermissionId.Should().Be(id);
    }

    [Fact]
    public void RolePermission_Should_Assign_RoleId()
    {
        var rolePermission = new RolePermission();
        var roleId = Guid.NewGuid();

        rolePermission.RoleId = roleId;

        rolePermission.RoleId.Should().Be(roleId);
    }

    [Fact]
    public void RolePermission_Should_Assign_PermissionId()
    {
        var rolePermission = new RolePermission();
        var permissionId = Guid.NewGuid();

        rolePermission.PermissionId = permissionId;

        rolePermission.PermissionId.Should().Be(permissionId);
    }

    [Fact]
    public void RolePermission_Should_Assign_All_Properties()
    {
        var rolePermissionId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        var rolePermission = new RolePermission
        {
            RolePermissionId = rolePermissionId,
            RoleId = roleId,
            PermissionId = permissionId
        };

        rolePermission.RolePermissionId.Should().Be(rolePermissionId);
        rolePermission.RoleId.Should().Be(roleId);
        rolePermission.PermissionId.Should().Be(permissionId);
    }

    [Fact]
    public void RolePermission_Should_Have_Default_Values()
    {
        var rolePermission = new RolePermission();

        rolePermission.RolePermissionId.Should().Be(Guid.Empty);
        rolePermission.RoleId.Should().Be(Guid.Empty);
        rolePermission.PermissionId.Should().Be(Guid.Empty);
    }
}