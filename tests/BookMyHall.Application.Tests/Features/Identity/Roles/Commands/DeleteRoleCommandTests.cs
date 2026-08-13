using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;
using FluentAssertions;
using MediatR;

namespace BookMyHall.Application.Tests.Features.Identity.Role.Commands;

public sealed class DeleteRoleCommandTests
{
    [Fact]
    public void Should_SetRoleId()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        // Act
        var command = new DeleteRoleCommand(roleId);

        // Assert
        command.RoleId.Should().Be(roleId);
    }

    [Fact]
    public void Should_ImplementCorrectRequestType()
    {
        // Arrange
        var command = new DeleteRoleCommand(Guid.NewGuid());

        // Assert
        command.Should()
            .BeAssignableTo<IRequest<ApiResponse<bool>>>();
    }

    [Fact]
    public void Should_CreateWithEmptyRoleId()
    {
        // Act
        var command = new DeleteRoleCommand(Guid.Empty);

        // Assert
        command.RoleId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Should_BeEqual_WhenRoleIdsAreEqual()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        var first = new DeleteRoleCommand(roleId);
        var second = new DeleteRoleCommand(roleId);

        // Assert
        first.Should().Be(second);
    }

    [Fact]
    public void Should_NotBeEqual_WhenRoleIdsAreDifferent()
    {
        // Arrange
        var first = new DeleteRoleCommand(Guid.NewGuid());
        var second = new DeleteRoleCommand(Guid.NewGuid());

        // Assert
        first.Should().NotBe(second);
    }
}