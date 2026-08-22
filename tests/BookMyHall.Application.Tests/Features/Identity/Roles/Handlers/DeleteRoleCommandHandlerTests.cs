using System.Net;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Features.Identity;
using DomainRole = BookMyHall.Domain.Entities.Identity.Role;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using FluentAssertions;
using Moq;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Tests.Features.Identity.Role;

public sealed class DeleteRoleCommandHandlerTests
{
    private readonly Mock<IRoleRepository> _roleRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMessageHelper> _messageHelperMock = new();
    private readonly DeleteRoleCommandHandler _handler;
    private readonly Mock<ICacheService> _cacheService=new();
    

    public DeleteRoleCommandHandlerTests()
    {
            _handler = new DeleteRoleCommandHandler(
            _roleRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _messageHelperMock.Object,
            _cacheService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var command = new DeleteRoleCommand(roleId);

        _roleRepositoryMock.Setup(x => x.GetByIdAsync(roleId,
                It.IsAny<CancellationToken>())).ReturnsAsync((DomainRole?)null);

        _messageHelperMock
            .Setup(x => x.NotFoundEntity(ResourceNames.Entities,EntityKeys.Role))
            .Returns("Role not found.");

        // Act
        var result = await _handler.Handle(command,CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Data.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        result.Message.Should().Be("Role not found.");

        _roleRepositoryMock.Verify(x => x.GetByIdAsync(roleId,
                It.IsAny<CancellationToken>()),Times.Once);

        _roleRepositoryMock.Verify(x => x.UpdateAsync(
                It.IsAny<DomainRole>(),It.IsAny<CancellationToken>()),Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDeactivateRole_WhenRoleExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new DomainRole
        {
            RoleId = roleId,
            RoleName = "Admin",
            IsActive = true
        };

       var command = new DeleteRoleCommand(roleId);

        _roleRepositoryMock.Setup(x => x.GetByIdAsync(roleId,
                It.IsAny<CancellationToken>())).ReturnsAsync(role);

        _roleRepositoryMock.Setup(x => x.UpdateAsync(role,
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _messageHelperMock.Setup(x => x.DeletedEntity(
                ResourceNames.Entities,EntityKeys.Role))
            .Returns("Role deleted successfully.");

        // Act
        var result = await _handler.Handle(command,CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Message.Should().Be("Role deleted successfully.");
        role.IsActive.Should().BeFalse();

        _roleRepositoryMock.Verify( x => x.GetByIdAsync(roleId,
                It.IsAny<CancellationToken>()),Times.Once);

        _roleRepositoryMock.Verify(x => x.UpdateAsync(role,
                It.IsAny<CancellationToken>()),Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),Times.Once);

        _messageHelperMock.Verify(x => x.DeletedEntity(
                ResourceNames.Entities,EntityKeys.Role),Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenRoleIsSuccessfullyDeleted()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new DomainRole
        {
            RoleId = roleId,
            RoleName = "Manager",
            IsActive = true
        };

    var command = new DeleteRoleCommand(roleId);
        SetupExistingRole(role);

        _messageHelperMock.Setup(x => x.DeletedEntity(
                ResourceNames.Entities,EntityKeys.Role))
            .Returns("Role deleted successfully.");

        // Act
        var result = await _handler.Handle(command,CancellationToken.None);

        // Assert
        result.Data.Should().BeTrue();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateOrSave_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var command = new DeleteRoleCommand(roleId);

        _roleRepositoryMock.Setup(x => x.GetByIdAsync(roleId,
                It.IsAny<CancellationToken>())).ReturnsAsync((DomainRole?)null);

        _messageHelperMock.Setup(x => x.NotFoundEntity(
                ResourceNames.Entities,EntityKeys.Role)).Returns("Role not found.");

        // Act
        await _handler.Handle(command,CancellationToken.None);

        // Assert
        _roleRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<DomainRole>(),
                It.IsAny<CancellationToken>()),Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),Times.Never);
    }

    private void SetupExistingRole(DomainRole role)
    {
        _roleRepositoryMock.Setup(x => x.GetByIdAsync(role.RoleId,
                It.IsAny<CancellationToken>())).ReturnsAsync(role);

        _roleRepositoryMock.Setup(x => x.UpdateAsync(role,
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }
}

