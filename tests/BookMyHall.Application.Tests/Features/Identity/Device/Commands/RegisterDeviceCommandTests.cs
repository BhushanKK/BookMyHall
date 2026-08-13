using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Tests.Features.Identity.Device.Commands;

public sealed class RegisterDeviceCommandTests
{
    [Fact]
    public void Should_Create_RegisterDeviceCommand()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var command = new RegisterDeviceCommand
        {
            DeviceId = Guid.NewGuid(),
            UserId = userId,
            DeviceIdentifier = "device-123",
            PushNotificationToken = "fcm-token",
            DeviceName = "Samsung Galaxy",
            DeviceType = "Mobile",
            OperatingSystem = "Android",
            Browser = "Chrome",
            AppVersion = "1.0.0",
            LastIpAddress = "192.168.1.10",
            LastLoginDate = DateTimeOffset.UtcNow,
            LastActivity = DateTimeOffset.UtcNow,
            IsTrusted = false,
            TrustedDate = null,
            IsActive = true
        };

        // Assert
        Assert.NotNull(command);
        Assert.NotEqual(Guid.Empty, command.DeviceId);
        Assert.Equal(userId, command.UserId);
        Assert.Equal("device-123", command.DeviceIdentifier);
        Assert.Equal("fcm-token", command.PushNotificationToken);
        Assert.Equal("Samsung Galaxy", command.DeviceName);
        Assert.Equal("Mobile", command.DeviceType);
        Assert.Equal("Android", command.OperatingSystem);
        Assert.Equal("Chrome", command.Browser);
        Assert.Equal("1.0.0", command.AppVersion);
        Assert.Equal("192.168.1.10", command.LastIpAddress);
        Assert.True(command.IsActive);
        Assert.False(command.IsTrusted);
        Assert.Null(command.TrustedDate);
    }

    [Fact]
    public void Should_Implement_IRequest_With_DeviceDto_Response()
    {
        // Arrange
        var command = new RegisterDeviceCommand();

        // Assert
        Assert.IsAssignableFrom<MediatR.IRequest<ApiResponse<DeviceDto>>>(command);
    }

    [Fact]
    public void Should_Set_DeviceIdentifier()
    {
        // Arrange
        var command = new RegisterDeviceCommand();

        // Act
        command.DeviceIdentifier = "unique-device-id";

        // Assert
        Assert.Equal("unique-device-id", command.DeviceIdentifier);
    }

    [Fact]
    public void Should_Set_UserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RegisterDeviceCommand();

        // Act
        command.UserId = userId;

        // Assert
        Assert.Equal(userId, command.UserId);
    }

    [Fact]
    public void Should_Set_PushNotificationToken()
    {
        // Arrange
        var command = new RegisterDeviceCommand();

        // Act
        command.PushNotificationToken = "push-token";

        // Assert
        Assert.Equal("push-token", command.PushNotificationToken);
    }

    [Fact]
    public void Should_Set_DeviceInformation()
    {
        // Arrange
        var command = new RegisterDeviceCommand();

        // Act
        command.DeviceName = "Chrome Browser";
        command.DeviceType = "Desktop";
        command.OperatingSystem = "Windows";
        command.Browser = "Chrome";
        command.AppVersion = "1.0.0";

        // Assert
        Assert.Equal("Chrome Browser", command.DeviceName);
        Assert.Equal("Desktop", command.DeviceType);
        Assert.Equal("Windows", command.OperatingSystem);
        Assert.Equal("Chrome", command.Browser);
        Assert.Equal("1.0.0", command.AppVersion);
    }

    [Fact]
    public void Should_Set_Activity_Information()
    {
        // Arrange
        var lastLoginDate = DateTimeOffset.UtcNow;
        var lastActivity = DateTimeOffset.UtcNow;

        var command = new RegisterDeviceCommand();

        // Act
        command.LastLoginDate = lastLoginDate;
        command.LastActivity = lastActivity;
        command.LastIpAddress = "127.0.0.1";

        // Assert
        Assert.Equal(lastLoginDate, command.LastLoginDate);
        Assert.Equal(lastActivity, command.LastActivity);
        Assert.Equal("127.0.0.1", command.LastIpAddress);
    }

    [Fact]
    public void Should_Set_Trusted_Status()
    {
        // Arrange
        var trustedDate = DateTimeOffset.UtcNow;
        var command = new RegisterDeviceCommand();

        // Act
        command.IsTrusted = true;
        command.TrustedDate = trustedDate;

        // Assert
        Assert.True(command.IsTrusted);
        Assert.Equal(trustedDate, command.TrustedDate);
    }

    [Fact]
    public void Should_Set_Active_Status()
    {
        // Arrange
        var command = new RegisterDeviceCommand();

        // Act
        command.IsActive = true;

        // Assert
        Assert.True(command.IsActive);
    }
}