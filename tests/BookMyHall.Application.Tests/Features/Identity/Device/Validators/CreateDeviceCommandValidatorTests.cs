using BookMyHall.Application.Features.Identity;
using BookMyHall.Application.Validations;
using BookMyHall.Shared.Localization;
using FluentAssertions;
using Moq;

namespace BookMyHall.Application.Tests.Validations;

public sealed class CreateDeviceCommandValidatorTests
{
    private readonly Mock<ILocalizationService> _localizerMock;
    private readonly CreateDeviceCommandValidator _validator;

    public CreateDeviceCommandValidatorTests()
    {
        _localizerMock = new Mock<ILocalizationService>();

        _localizerMock
            .Setup(x => x.Get(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns((string resourceName, string key) => key);

        _localizerMock
            .Setup(x => x.Get(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<object[]>()))
            .Returns((string resourceName, string key, object[] arguments) => key);

        _validator = new CreateDeviceCommandValidator(_localizerMock.Object);
    }

    [Fact]
    public async Task Should_Pass_When_CommandIsValid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Fail_When_UserIdIsEmpty()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UserId = Guid.Empty;

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(RegisterDeviceCommand.UserId));
    }

    [Fact]
    public async Task Should_Fail_When_DeviceIdentifierIsEmpty()
    {
        // Arrange
        var command = CreateValidCommand();
        command.DeviceIdentifier = string.Empty;

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(RegisterDeviceCommand.DeviceIdentifier));
    }

    [Fact]
    public async Task Should_Fail_When_DeviceIdentifierExceedsMaximumLength()
    {
        // Arrange
        var command = CreateValidCommand();
        command.DeviceIdentifier = new string('A', 251);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(RegisterDeviceCommand.DeviceIdentifier));
    }

    [Fact]
    public async Task Should_Fail_When_DeviceTypeIsEmpty()
    {
        // Arrange
        var command = CreateValidCommand();
        command.DeviceType = string.Empty;

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(RegisterDeviceCommand.DeviceType));
    }

    [Fact]
    public async Task Should_Fail_When_DeviceTypeExceedsMaximumLength()
    {
        // Arrange
        var command = CreateValidCommand();
        command.DeviceType = new string('A', 51);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(RegisterDeviceCommand.DeviceType));
    }

    [Fact]
    public async Task Should_Fail_When_PushNotificationTokenExceedsMaximumLength()
    {
        // Arrange
        var command = CreateValidCommand();
        command.PushNotificationToken = new string('A', 501);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(RegisterDeviceCommand.PushNotificationToken));
    }

    [Fact]
    public async Task Should_Fail_When_DeviceNameExceedsMaximumLength()
    {
        // Arrange
        var command = CreateValidCommand();
        command.DeviceName = new string('A', 101);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(RegisterDeviceCommand.DeviceName));
    }

    [Fact]
    public async Task Should_Fail_When_OperatingSystemExceedsMaximumLength()
    {
        // Arrange
        var command = CreateValidCommand();
        command.OperatingSystem = new string('A', 101);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(RegisterDeviceCommand.OperatingSystem));
    }

    [Fact]
    public async Task Should_Fail_When_BrowserExceedsMaximumLength()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Browser = new string('A', 101);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(RegisterDeviceCommand.Browser));
    }

    [Fact]
    public async Task Should_Fail_When_AppVersionExceedsMaximumLength()
    {
        // Arrange
        var command = CreateValidCommand();
        command.AppVersion = new string('A', 51);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(RegisterDeviceCommand.AppVersion));
    }

    [Fact]
    public async Task Should_Fail_When_LastIpAddressExceedsMaximumLength()
    {
        // Arrange
        var command = CreateValidCommand();
        command.LastIpAddress = new string('A', 101);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(x => x.PropertyName == nameof(RegisterDeviceCommand.LastIpAddress));
    }

    [Fact]
    public async Task Should_Pass_When_OptionalPropertiesAreEmpty()
    {
        // Arrange
        var command = CreateValidCommand();

        command.PushNotificationToken = string.Empty;
        command.DeviceName = string.Empty;
        command.OperatingSystem = string.Empty;
        command.Browser = string.Empty;
        command.AppVersion = string.Empty;
        command.LastIpAddress = string.Empty;

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Pass_When_PropertiesAreExactlyMaximumLength()
    {
        // Arrange
        var command = CreateValidCommand();

        command.DeviceIdentifier = new string('A', 250);
        command.DeviceType = new string('A', 50);
        command.PushNotificationToken = new string('A', 500);
        command.DeviceName = new string('A', 100);
        command.OperatingSystem = new string('A', 100);
        command.Browser = new string('A', 100);
        command.AppVersion = new string('A', 50);
        command.LastIpAddress = new string('A', 100);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    private static RegisterDeviceCommand CreateValidCommand()
    {
        return new RegisterDeviceCommand
        {
            UserId = Guid.NewGuid(),
            DeviceIdentifier = "device-123",
            DeviceType = "Mobile",
            PushNotificationToken = "push-token",
            DeviceName = "Test Device",
            OperatingSystem = "Android",
            Browser = "Chrome",
            AppVersion = "1.0.0",
            LastIpAddress = "192.168.1.10"
        };
    }
}