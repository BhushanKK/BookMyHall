using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Tests.Features.Identity.Device.Commands;

public sealed class UpdateDeviceCommandTests
{
    [Fact]
    public void Should_Create_UpdateDeviceCommand_With_Valid_Properties()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var command = new UpdateDeviceCommand
        {
            DeviceId = deviceId,
            UserId = userId,
            DeviceIdentifier = "device-001",
            PushNotificationToken = "push-token",
            DeviceName = "Chrome Desktop",
            DeviceType = "Desktop",
            OperatingSystem = "Windows",
            Browser = "Chrome",
            AppVersion = "1.0.0",
            LastIpAddress = "127.0.0.1",
            LastLoginDate = DateTimeOffset.UtcNow,
            LastActivity = DateTimeOffset.UtcNow,
            IsTrusted = true,
            TrustedDate = DateTimeOffset.UtcNow,
            IsActive = true
        };

        // Assert
        Assert.Equal(deviceId, command.DeviceId);
        Assert.Equal(userId, command.UserId);
        Assert.Equal("device-001", command.DeviceIdentifier);
        Assert.Equal("push-token", command.PushNotificationToken);
        Assert.Equal("Chrome Desktop", command.DeviceName);
        Assert.Equal("Desktop", command.DeviceType);
        Assert.Equal("Windows", command.OperatingSystem);
        Assert.Equal("Chrome", command.Browser);
        Assert.Equal("1.0.0", command.AppVersion);
        Assert.Equal("127.0.0.1", command.LastIpAddress);
        Assert.True(command.IsTrusted);
        Assert.True(command.IsActive);
    }

    [Fact]
    public void Should_Create_UpdateDeviceCommand_With_Nullable_Properties()
    {
        // Act
        var command = new UpdateDeviceCommand
        {
            DeviceId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DeviceIdentifier = "device-001",
            DeviceType = "Mobile",
            PushNotificationToken = string.Empty,
            DeviceName = string.Empty,
            OperatingSystem = string.Empty,
            Browser = string.Empty,
            AppVersion = string.Empty,
            LastIpAddress = string.Empty,
            LastActivity = null,
            TrustedDate = null
        };

        // Assert
        Assert.Equal(string.Empty, command.PushNotificationToken);
        Assert.Equal(string.Empty, command.DeviceName);
        Assert.Equal(string.Empty, command.OperatingSystem);
        Assert.Equal(string.Empty, command.Browser);
        Assert.Equal(string.Empty, command.AppVersion);
        Assert.Equal(string.Empty, command.LastIpAddress);
        Assert.Null(command.LastActivity);
        Assert.Null(command.TrustedDate);
    }

    [Fact]
    public void Should_Allow_IsTrusted_To_Be_Updated()
    {
        // Arrange
        var command = new UpdateDeviceCommand
        {
            DeviceId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DeviceIdentifier = "device-001",
            DeviceType = "Desktop",
            IsTrusted = false
        };

        // Act
        command.IsTrusted = true;

        // Assert
        Assert.True(command.IsTrusted);
    }

    [Fact]
    public void Should_Allow_IsActive_To_Be_Updated()
    {
        // Arrange
        var command = new UpdateDeviceCommand
        {
            DeviceId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DeviceIdentifier = "device-001",
            DeviceType = "Desktop",
            IsActive = true
        };

        // Act
        command.IsActive = false;

        // Assert
        Assert.False(command.IsActive);
    }

    [Fact]
    public void Should_Implement_IRequest_With_DeviceDto_Response()
    {
        // Arrange
        var command = new UpdateDeviceCommand();

        // Assert
        Assert.IsAssignableFrom<IRequest<ApiResponse<DeviceDto>>>(command);
    }
}