using FluentAssertions;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class UserRoleTests
{
    [Fact]
    public void UserRole_Should_Assign_UserRoleId()
    {
        var userRole = new UserRole();
        var id = Guid.NewGuid();

        userRole.UserRoleId = id;

        userRole.UserRoleId.Should().Be(id);
    }

    [Fact]
    public void UserRole_Should_Assign_UserId()
    {
        var userRole = new UserRole();
        var userId = Guid.NewGuid();

        userRole.UserId = userId;

        userRole.UserId.Should().Be(userId);
    }

    [Fact]
    public void UserRole_Should_Assign_RoleId()
    {
        var userRole = new UserRole();
        var roleId = Guid.NewGuid();

        userRole.RoleId = roleId;

        userRole.RoleId.Should().Be(roleId);
    }

    [Fact]
    public void UserRole_Should_Assign_All_Properties()
    {
        var userRoleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var userRole = new UserRole
        {
            UserRoleId = userRoleId,
            UserId = userId,
            RoleId = roleId
        };

        userRole.UserRoleId.Should().Be(userRoleId);
        userRole.UserId.Should().Be(userId);
        userRole.RoleId.Should().Be(roleId);
    }

    [Fact]
    public void UserRole_Should_Have_Default_Empty_Guids()
    {
        var userRole = new UserRole();

        userRole.UserRoleId.Should().Be(Guid.Empty);
        userRole.UserId.Should().Be(Guid.Empty);
        userRole.RoleId.Should().Be(Guid.Empty);
    }
}