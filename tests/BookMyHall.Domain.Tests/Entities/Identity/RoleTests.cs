using FluentAssertions;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class RoleTests
{
    [Fact]
    public void Role_Should_Be_Active_By_Default()
    {
        var role = new Role();
        role.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_Should_Set_IsActive_To_False()
    {
        var role = new Role();
        role.Deactivate();
        role.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Role_Should_Assign_RoleName()
    {
        var role = new Role();
        role.RoleName = "Administrator";
        role.RoleName.Should().Be("Administrator");
    }

    [Fact]
    public void Role_Should_Assign_RoleId()
    {
        var role = new Role();
        var id = Guid.NewGuid();
        role.RoleId = id;
        role.RoleId.Should().Be(id);
    }

    [Fact]
    public void Deactivate_Should_Not_Change_RoleId_Or_RoleName()
    {
        var id = Guid.NewGuid();
        var role = new Role
        {
            RoleId = id,
            RoleName = "Administrator"
        };

        role.Deactivate();
        role.RoleId.Should().Be(id);
        role.RoleName.Should().Be("Administrator");
        role.IsActive.Should().BeFalse();
    }
}