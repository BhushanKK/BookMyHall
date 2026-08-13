using System.Net;
using AutoMapper;
using FluentAssertions;
using Moq;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using DomainRole = BookMyHall.Domain.Entities.Identity.Role;

namespace BookMyHall.Application.Tests.Features.Identity.Role;

public sealed class GetRoleByIdQueryHandlerTests
{
    private readonly Mock<IRoleRepository> _roleRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessageHelper> _messageHelperMock = new();
    private readonly GetRoleByIdQueryHandler _handler;

    public GetRoleByIdQueryHandlerTests()
    {
            _handler = new GetRoleByIdQueryHandler(
            _roleRepositoryMock.Object,
            _mapperMock.Object,
            _messageHelperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnRole_WhenRoleExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var query = new GetRoleByIdQuery(roleId);
        var role = CreateRole(roleId);
        var mappedRole = CreateRole(roleId);

        _roleRepositoryMock
            .Setup(x => x.GetByIdAsync(
                roleId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        _mapperMock
            .Setup(x => x.Map<DomainRole>(role))
            .Returns(mappedRole);

        _messageHelperMock
            .Setup(x => x.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Role retrieved successfully.");

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Message.Should().Be("Role retrieved successfully.");
        result.Data.Should().BeSameAs(mappedRole);

        _roleRepositoryMock.Verify(
            x => x.GetByIdAsync(
                roleId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<DomainRole>(role),
            Times.Once);

        _messageHelperMock.Verify(
            x => x.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Role),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        var query = new GetRoleByIdQuery(roleId);

        _roleRepositoryMock
            .Setup(x => x.GetByIdAsync(
                roleId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainRole?)null);

        _messageHelperMock
            .Setup(x => x.NotFoundEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Role not found.");

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        result.Message.Should().Be("Role not found.");
        result.Data.Should().BeNull();

        _roleRepositoryMock.Verify(
            x => x.GetByIdAsync(
                roleId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<DomainRole>(
                It.IsAny<DomainRole>()),
            Times.Never);

        _messageHelperMock.Verify(
            x => x.NotFoundEntity(
                ResourceNames.Entities,
                EntityKeys.Role),
            Times.Once);

        _messageHelperMock.Verify(
            x => x.RetrievedEntity(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationTokenToRepository()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        var query = new GetRoleByIdQuery(roleId);

        var cancellationToken = new CancellationTokenSource().Token;

        var role = CreateRole(roleId);

        _roleRepositoryMock
            .Setup(x => x.GetByIdAsync(
                roleId,
                cancellationToken))
            .ReturnsAsync(role);

        _mapperMock
            .Setup(x => x.Map<DomainRole>(role))
            .Returns(role);

        _messageHelperMock
            .Setup(x => x.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Role retrieved successfully.");

        // Act
        await _handler.Handle(
            query,
            cancellationToken);

        // Assert
        _roleRepositoryMock.Verify(
            x => x.GetByIdAsync(
                roleId,
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotMapRole_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        var query = new GetRoleByIdQuery(roleId);

        _roleRepositoryMock
            .Setup(x => x.GetByIdAsync(
                roleId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainRole?)null);

        _messageHelperMock
            .Setup(x => x.NotFoundEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Role not found.");

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();

        _mapperMock.Verify(
            x => x.Map<DomainRole>(
                It.IsAny<DomainRole>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedRole_WhenRoleExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        var query = new GetRoleByIdQuery(roleId);

        var databaseRole = CreateRole(
            roleId,
            "Administrator");

        var mappedRole = CreateRole(
            roleId,
            "Administrator");

        _roleRepositoryMock
            .Setup(x => x.GetByIdAsync(
                roleId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(databaseRole);

        _mapperMock
            .Setup(x => x.Map<DomainRole>(databaseRole))
            .Returns(mappedRole);

        _messageHelperMock
            .Setup(x => x.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Role retrieved successfully.");

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        result.Data.Should().BeSameAs(mappedRole);
        result.Data.Should().NotBeSameAs(databaseRole);
    }

    private static DomainRole CreateRole(
        Guid? roleId = null,
        string roleName = "Admin")
    {
        return new DomainRole
        {
            RoleId = roleId ?? Guid.NewGuid(),
            RoleName = roleName,
            IsActive = true
        };
    }
}