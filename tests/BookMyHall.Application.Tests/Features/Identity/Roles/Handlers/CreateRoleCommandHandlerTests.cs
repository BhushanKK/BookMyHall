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

public sealed class CreateRoleCommandHandlerTests
{
    private readonly Mock<IRoleRepository> _roleRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IValidator<CreateRoleCommand>> _validatorMock = new();
    private readonly Mock<IMessageHelper> _messageHelperMock = new();
    private readonly Mock<ICacheService> _cacheService=new();

    private readonly CreateRoleCommandHandler _handler;

    public CreateRoleCommandHandlerTests()
    {
        _handler = new CreateRoleCommandHandler(
            _roleRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _messageHelperMock.Object,
            _cacheService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var command = CreateCommand();

        var validationResult = new ValidationResult(
        [
            new ValidationFailure(
                nameof(CreateRoleCommand.RoleName),
                "Role name is required.")
        ]);

        SetupValidation(command, validationResult);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        result.Message.Should().Contain("Role name is required.");

        _mapperMock.Verify(
            x => x.Map<DomainRole>(
                It.IsAny<CreateRoleCommand>()),
            Times.Never);

        _roleRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<DomainRole>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateRole_WhenRequestIsValid()
    {
        // Arrange
        var command = CreateCommand();

        var role = new DomainRole
        {
            RoleName = command.RoleName
        };

        var roleDto = new RoleDto
        {
            RoleName = command.RoleName
        };

        SetupValidation(command, new ValidationResult());

        _mapperMock
            .Setup(x => x.Map<DomainRole>(command))
            .Returns(role);

        _roleRepositoryMock
            .Setup(x => x.AddAsync(
                role,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<RoleDto>(role))
            .Returns(roleDto);

        _messageHelperMock
            .Setup(x => x.AddedEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Role added successfully.");

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        result.Data.Should().BeSameAs(roleDto);
        result.Message.Should().Be("Role added successfully.");

        _validatorMock.Verify(
            x => x.ValidateAsync(
                command,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<DomainRole>(command),
            Times.Once);

        _roleRepositoryMock.Verify(
            x => x.AddAsync(
                role,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<RoleDto>(role),
            Times.Once);

        _messageHelperMock.Verify(
            x => x.AddedEntity(
                ResourceNames.Entities,
                EntityKeys.Role),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenDuplicateRoleExists()
    {
        // Arrange
        var command = CreateCommand();

        var role = new DomainRole
        {
            RoleName = command.RoleName
        };

        SetupValidation(command, new ValidationResult());

        _mapperMock
            .Setup(x => x.Map<DomainRole>(command))
            .Returns(role);

        _roleRepositoryMock
            .Setup(x => x.AddAsync(
                role,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DuplicateRecordException());

        _messageHelperMock
            .Setup(x => x.AlreadyExistsEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Role already exists.");

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
        result.Message.Should().Be("Role already exists.");

        _validatorMock.Verify(
            x => x.ValidateAsync(
                command,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<DomainRole>(command),
            Times.Once);

        _roleRepositoryMock.Verify(
            x => x.AddAsync(
                role,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<RoleDto>(
                It.IsAny<DomainRole>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotCreateRole_WhenValidationFails()
    {
        // Arrange
        var command = CreateCommand();

        var validationResult = new ValidationResult(
        [
            new ValidationFailure(
                nameof(CreateRoleCommand.RoleName),
                "Role name is required.")
        ]);

        SetupValidation(command, validationResult);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();

        _mapperMock.Verify(
            x => x.Map<DomainRole>(
                It.IsAny<CreateRoleCommand>()),
            Times.Never);

        _roleRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_ShouldNotSaveChanges_WhenAddRoleFails()
    {
        // Arrange
        var command = CreateCommand();

        var role = new DomainRole
        {
            RoleName = command.RoleName
        };

        SetupValidation(command, new ValidationResult());

        _mapperMock
            .Setup(x => x.Map<DomainRole>(command))
            .Returns(role);

        _roleRepositoryMock
            .Setup(x => x.AddAsync(
                role,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DuplicateRecordException());

        _messageHelperMock
            .Setup(x => x.AlreadyExistsEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Role already exists.");

        // Act
        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static CreateRoleCommand CreateCommand()
    {
        return new CreateRoleCommand
        {
            RoleName = "Admin"
        };
    }

    private void SetupValidation(
        CreateRoleCommand command,
        ValidationResult result)
    {
        _validatorMock
            .Setup(x => x.ValidateAsync(
                command,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
    }
}
