using BookMyHall.Application.Features.Identity;
using FluentAssertions;

namespace BookMyHall.Application.Tests.Features.Identity.Role.Queries;

public sealed class GetRoleByIdQueryTests
{
    [Fact]
    public void Should_SetRoleId()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        // Act
        var query = new GetRoleByIdQuery(roleId);

        // Assert
        query.RoleId.Should().Be(roleId);
    }

    

    [Fact]
    public void Should_CreateWithEmptyRoleId()
    {
        // Act
        var query = new GetRoleByIdQuery(Guid.Empty);

        // Assert
        query.RoleId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Should_BeEqual_WhenRoleIdsAreEqual()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        var first = new GetRoleByIdQuery(roleId);
        var second = new GetRoleByIdQuery(roleId);

        // Assert
        first.Should().Be(second);
    }

    [Fact]
    public void Should_NotBeEqual_WhenRoleIdsAreDifferent()
    {
        // Arrange
        var first = new GetRoleByIdQuery(Guid.NewGuid());
        var second = new GetRoleByIdQuery(Guid.NewGuid());

        // Assert
        first.Should().NotBe(second);
    }

    [Fact]
    public void Should_BeRecordWithValueEquality()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        var query1 = new GetRoleByIdQuery(roleId);
        var query2 = query1 with { };

        // Assert
        query1.Should().Be(query2);
    }
}