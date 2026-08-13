using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;
using FluentAssertions;
using MediatR;

namespace BookMyHall.Application.Tests.Features.Identity.Role.Commands;

public sealed class CreateRoleCommandTests
{
    [Fact]
    public void Should_InheritFromRoleDto()
    {
        // Arrange
        var command = new CreateRoleCommand();

        // Assert
        command.Should().BeAssignableTo<RoleDto>();
    }

    [Fact]
    public void Should_ImplementIRequestWithCorrectResponseType()
    {
        // Arrange
        var command = new CreateRoleCommand();

        // Assert
        command.Should()
            .BeAssignableTo<IRequest<ApiResponse<RoleDto>>>();
    }

    [Fact]
    public void Should_HaveDefaultValues()
    {
        // Act
        var command = new CreateRoleCommand();

        // Assert
        command.RoleId.Should().Be(Guid.Empty);
        command.RoleName.Should().BeEmpty();
        command.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Should_SetAndGetRoleId()
    {
        // Arrange
        var command = new CreateRoleCommand();
        var roleId = Guid.NewGuid();

        // Act
        command.RoleId = roleId;

        // Assert
        command.RoleId.Should().Be(roleId);
    }

    [Fact]
    public void Should_SetRoleNameDuringInitialization()
    {
        // Arrange
        const string roleName = "Administrator";

        // Act
        var command = new CreateRoleCommand
        {
            RoleName = roleName
        };

        // Assert
        command.RoleName.Should().Be(roleName);
    }

    [Fact]
    public void Should_SetIsActiveDuringInitialization()
    {
        // Arrange
        var command = new CreateRoleCommand
        {
            IsActive = true
        };

        // Assert
        command.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Should_SetAllPropertiesDuringInitialization()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        // Act
        var command = new CreateRoleCommand
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
}