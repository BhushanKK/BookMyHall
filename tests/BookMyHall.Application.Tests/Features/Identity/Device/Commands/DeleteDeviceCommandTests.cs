using BookMyHall.Application.Features.Identity;
using FluentAssertions;
using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Tests.Features.Identity.Device.Commands;

public sealed class DeleteDeviceCommandTests
{
    [Fact]
    public void Should_Create_DeleteDeviceCommand()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string deviceIdentifier = "device-123";

        // Act
        var command = new DeleteDeviceCommand(userId,deviceIdentifier);

        // Assert
        command.Should().NotBeNull();
        command.UserId.Should().Be(userId);
        command.DeviceIdentifier.Should().Be(deviceIdentifier);
    }

    [Fact]
    public void Should_Set_UserId()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var command = new DeleteDeviceCommand(userId,"device-123");

        // Assert
        command.UserId.Should().Be(userId);
    }

    [Fact]
    public void Should_Set_DeviceIdentifier()
    {
        // Arrange
        const string deviceIdentifier = "device-123";

        // Act
        var command = new DeleteDeviceCommand(Guid.NewGuid(),deviceIdentifier);

        // Assert
        command.DeviceIdentifier.Should().Be(deviceIdentifier);
    }

    [Fact]
    public void Should_Implement_IRequest_With_Boolean_Response()
    {
        // Arrange
        var command = new DeleteDeviceCommand(Guid.NewGuid(),"device-123");

        // Assert
        command.Should().BeAssignableTo<IRequest<ApiResponse<bool>>>();
    }

    [Fact]
    public void Should_Preserve_Command_Values()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string deviceIdentifier = "android-device-001";

        // Act
        var command = new DeleteDeviceCommand(userId,deviceIdentifier);

        // Assert
        command.UserId.Should().Be(userId);
        command.DeviceIdentifier.Should().Be(deviceIdentifier);
    }

    [Fact]
    public void Should_Support_Record_Value_Equality()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command1 = new DeleteDeviceCommand(userId,"device-123");
        var command2 = new DeleteDeviceCommand(userId,"device-123");

        // Assert
        command1.Should().Be(command2);
    }

    [Fact]
    public void Should_Not_Be_Equal_When_UserId_Is_Different()
    {
        // Arrange
        var command1 = new DeleteDeviceCommand(Guid.NewGuid(),"device-123");
        var command2 = new DeleteDeviceCommand(Guid.NewGuid(),"device-123");

        // Assert
        command1.Should().NotBe(command2);
    }

    [Fact]
    public void Should_Not_Be_Equal_When_DeviceIdentifier_Is_Different()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command1 = new DeleteDeviceCommand(userId,"device-123");
        var command2 = new DeleteDeviceCommand(userId,"device-456");

        // Assert
        command1.Should().NotBe(command2);
    }
}