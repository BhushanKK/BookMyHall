using BookMyHall.Application.Features.Identity;
using BookMyHall.Application.Validations;
using BookMyHall.Shared.Localization;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;

namespace BookMyHall.Application.Tests.Features.Identity.Roles.Validators;

public sealed class CreateRoleCommandValidatorTests
{
    private readonly Mock<ILocalizationService> _localizerMock;
    private readonly CreateRoleCommandValidator _validator;

    public CreateRoleCommandValidatorTests()
    {
        _localizerMock = new Mock<ILocalizationService>();

        _localizerMock
            .Setup(x => x.Get(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()))
            .Returns("Validation error.");

        _validator = new CreateRoleCommandValidator(
            _localizerMock.Object);
    }

    [Fact]
    public void Should_Pass_WhenRoleNameIsValid()
    {
        // Arrange
        var command = new CreateRoleCommand
        {
            RoleName = "Admin"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RoleName);
    }

    [Fact]
    public void Should_Fail_WhenRoleNameIsNull()
    {
        // Arrange
        var command = new CreateRoleCommand
        {
            RoleName = null!
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoleName);
    }

    [Fact]
    public void Should_Fail_WhenRoleNameIsEmpty()
    {
        // Arrange
        var command = new CreateRoleCommand
        {
            RoleName = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoleName);
    }

    [Fact]
    public void Should_Fail_WhenRoleNameContainsOnlyWhitespace()
    {
        // Arrange
        var command = new CreateRoleCommand
        {
            RoleName = "   "
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoleName);
    }

    [Fact]
    public void Should_Pass_WhenRoleNameHasExactly20Characters()
    {
        // Arrange
        var command = new CreateRoleCommand
        {
            RoleName = new string('A', 20)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RoleName);
    }

    [Fact]
    public void Should_Fail_WhenRoleNameExceeds20Characters()
    {
        // Arrange
        var command = new CreateRoleCommand
        {
            RoleName = new string('A', 21)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoleName);
    }

    [Fact]
    public void Should_Pass_WhenRoleNameHasOneCharacter()
    {
        // Arrange
        var command = new CreateRoleCommand
        {
            RoleName = "A"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RoleName);
    }

    
    [Fact]
    public void Should_HaveValidationErrorOnlyForRoleName_WhenRoleNameIsInvalid()
    {
        // Arrange
        var command = new CreateRoleCommand
        {
            RoleName = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .OnlyContain(error =>
                error.PropertyName == nameof(CreateRoleCommand.RoleName));
    }
}

