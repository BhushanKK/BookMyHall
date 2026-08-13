using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;
using FluentAssertions;
using MediatR;

namespace BookMyHall.Application.Tests.Features.Identity.Role.Commands;

public sealed class UpdateRoleCommandTests
{
    [Fact]
    public void Should_InheritFromRoleDto()
    {
        // Arrange
        var command = new UpdateRoleCommand();

        // Assert
        command.Should().BeAssignableTo<RoleDto>();
    }

    [Fact]
    public void Should_ImplementCorrectRequestType()
    {
        // Arrange
        var command = new UpdateRoleCommand();

        // Assert
        command.Should()
            .BeAssignableTo<IRequest<ApiResponse<RoleDto>>>();
    }

    [Fact]
    public void Should_HaveDefaultValues()
    {
        // Act
        var command = new UpdateRoleCommand();

        // Assert
        command.RoleId.Should().Be(Guid.Empty);
        command.RoleName.Should().BeEmpty();
        command.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Should_SetRoleId()
    {
        // Arrange
        var command = new UpdateRoleCommand();
        var roleId = Guid.NewGuid();

        // Act
        command.RoleId = roleId;

        // Assert
        command.RoleId.Should().Be(roleId);
    }

    [Fact]
    public void Should_SetRolePropertiesDuringInitialization()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        // Act
        var command = new UpdateRoleCommand
        {
            RoleId = roleId,
            RoleName = "Administrator",
            IsActive = true
        };

        // Assert
        command.RoleId.Should().Be(roleId);
        command.RoleName.Should().Be("Administrator");
        command.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Should_AllowInactiveRole()
    {
        // Arrange
        var command = new UpdateRoleCommand
        {
            RoleId = Guid.NewGuid(),
            RoleName = "User",
            IsActive = false
        };

        // Assert
        command.IsActive.Should().BeFalse();
    }
}