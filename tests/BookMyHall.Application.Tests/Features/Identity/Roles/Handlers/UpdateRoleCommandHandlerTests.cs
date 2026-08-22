using System.Net;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

using DomainRole = BookMyHall.Domain.Entities.Identity.Role;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Tests.Features.Identity.Role;

public sealed class UpdateRoleCommandHandlerTests
{
    private readonly Mock<IRoleRepository> _roleRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IValidator<UpdateRoleCommand>> _validatorMock = new();
    private readonly Mock<IMessageHelper> _messageHelperMock = new();
        private readonly Mock<ICacheService> _cacheService=new();
    private readonly UpdateRoleCommandHandler _handler;

    public UpdateRoleCommandHandlerTests()
    {
            _handler = new UpdateRoleCommandHandler(
            _roleRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _messageHelperMock.Object,_cacheService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var command = CreateCommand();
        var validationResult = new ValidationResult(
        [
            new ValidationFailure(nameof(UpdateRoleCommand.RoleName),"Role name is required.")
        ]);

        SetupValidation(command, validationResult);

        // Act
        var result = await _handler.Handle(command,CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        result.Message.Should().Be("Role name is required.");

        _roleRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),Times.Never);

        _mapperMock.Verify(x => x.Map(It.IsAny<UpdateRoleCommand>(),
                It.IsAny<DomainRole>()),Times.Never);

        _roleRepositoryMock.Verify(x => x.UpdateAsync(
                It.IsAny<DomainRole>(),It.IsAny<CancellationToken>()),Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRoleDoesNotExist()
    {
        // Arrange
        var command = CreateCommand();
        SetupValidation(command);

        _roleRepositoryMock.Setup(x => x.GetByIdAsync(command.RoleId,
                It.IsAny<CancellationToken>())).ReturnsAsync((DomainRole?)null);

        _messageHelperMock.Setup(x => x.NotFoundEntity(
                ResourceNames.Entities,EntityKeys.Role))
            .Returns("Role not found.");

        // Act
        var result = await _handler.Handle(command,CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        result.Message.Should().Be("Role not found.");

        _validatorMock.Verify(x => x.ValidateAsync(command,
                It.IsAny<CancellationToken>()),Times.Once);

        _roleRepositoryMock.Verify(x => x.GetByIdAsync(command.RoleId,
                It.IsAny<CancellationToken>()),Times.Once);

        _mapperMock.Verify(x => x.Map(It.IsAny<UpdateRoleCommand>(),
                It.IsAny<DomainRole>()),Times.Never);

        _roleRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<DomainRole>(),
                It.IsAny<CancellationToken>()),Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenDuplicateRoleExists()
    {
        // Arrange
        var command = CreateCommand();
        var role = CreateRole(command.RoleId);
        SetupValidation(command);

        _roleRepositoryMock.Setup(x => x.GetByIdAsync(command.RoleId,
                It.IsAny<CancellationToken>())).ReturnsAsync(role);

        _mapperMock.Setup(x => x.Map(command, role));

        _roleRepositoryMock.Setup(x => x.UpdateAsync(role,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DuplicateRecordException());

        _messageHelperMock.Setup(x => x.AlreadyExistsEntity(
                ResourceNames.Entities,EntityKeys.Role))
            .Returns("Role already exists.");

        // Act
        var result = await _handler.Handle(command,CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
        result.Message.Should().Be("Role already exists.");

        _validatorMock.Verify(x => x.ValidateAsync(command,
                It.IsAny<CancellationToken>()),Times.Once);

        _roleRepositoryMock.Verify(x => x.GetByIdAsync(command.RoleId,
                It.IsAny<CancellationToken>()),Times.Once);

        _mapperMock.Verify(x => x.Map(command, role),Times.Once);

        _roleRepositoryMock.Verify(x => x.UpdateAsync(role,
                It.IsAny<CancellationToken>()),Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),Times.Never);

        _mapperMock.Verify(x => x.Map<RoleDto>(
                It.IsAny<DomainRole>()),Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateRole_WhenRequestIsValid()
    {
        // Arrange
        var command = CreateCommand();
        var role = CreateRole(command.RoleId);
        var roleDto = new RoleDto
        {
            RoleId = command.RoleId,
            RoleName = command.RoleName,
            IsActive = true
        };

        SetupValidation(command);

        _roleRepositoryMock.Setup(x => x.GetByIdAsync(command.RoleId,
                It.IsAny<CancellationToken>())).ReturnsAsync(role);

        _mapperMock.Setup(x => x.Map(command, role));

        _roleRepositoryMock.Setup(x => x.UpdateAsync(role,
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mapperMock.Setup(x => x.Map<RoleDto>(role)).Returns(roleDto);

        _messageHelperMock.Setup(x => x.UpdatedEntity(
                ResourceNames.Entities,EntityKeys.Role))
            .Returns("Role updated successfully.");

        // Act
        var result = await _handler.Handle(command,CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Data.Should().BeSameAs(roleDto);
        result.Message.Should().Be("Role updated successfully.");

        _validatorMock.Verify(x => x.ValidateAsync(command,
                It.IsAny<CancellationToken>()),Times.Once);

        _roleRepositoryMock.Verify(x => x.GetByIdAsync(command.RoleId,
                It.IsAny<CancellationToken>()),Times.Once);

        _mapperMock.Verify(x => x.Map(command, role),Times.Once);

        _roleRepositoryMock.Verify(x => x.UpdateAsync( role,
                It.IsAny<CancellationToken>()),Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),Times.Once);

        _mapperMock.Verify(x => x.Map<RoleDto>(role),Times.Once);

        _messageHelperMock.Verify(x => x.UpdatedEntity(
                ResourceNames.Entities,EntityKeys.Role),Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotAccessRepository_WhenValidationFails()
    {
        // Arrange
        var command = CreateCommand();
        var validationResult = new ValidationResult(
        [
            new ValidationFailure(nameof(UpdateRoleCommand.RoleName),"Role name is required.")
        ]);
        SetupValidation(command, validationResult);

        // Act
        await _handler.Handle(command,CancellationToken.None);

        // Assert
        _roleRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),Times.Never);

        _roleRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<DomainRole>(),
                It.IsAny<CancellationToken>()),Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotSaveChanges_WhenRoleDoesNotExist()
    {
        // Arrange
        var command = CreateCommand();
        SetupValidation(command);

        _roleRepositoryMock.Setup(x => x.GetByIdAsync(command.RoleId,
                It.IsAny<CancellationToken>())).ReturnsAsync((DomainRole?)null);

        _messageHelperMock.Setup(x => x.NotFoundEntity(
                ResourceNames.Entities,EntityKeys.Role))
            .Returns("Role not found.");

        // Act
        await _handler.Handle(command,CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),Times.Never);
    }

    private static UpdateRoleCommand CreateCommand()
    {
        return new UpdateRoleCommand
        {
            RoleId = Guid.NewGuid(),
            RoleName = "Updated Role",
            IsActive = true
        };
    }

    private static DomainRole CreateRole(Guid roleId)
    {
        return new DomainRole
        {
            RoleId = roleId,
            RoleName = "Old Role",
            IsActive = true
        };
    }

    private void SetupValidation(UpdateRoleCommand command, ValidationResult? validationResult = null)
    {
        _validatorMock.Setup(x => x.ValidateAsync(command,
                It.IsAny<CancellationToken>())).ReturnsAsync(
                validationResult ?? new ValidationResult());
    }
}