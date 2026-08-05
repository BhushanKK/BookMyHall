using FluentAssertions;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class DeviceTests
{
    [Fact]
    public void Device_Should_Assign_DeviceId()
    {
        var device = new Device();
        var id = Guid.NewGuid();

        device.DeviceId = id;

        device.DeviceId.Should().Be(id);
    }

    [Fact]
    public void Device_Should_Assign_UserId()
    {
        var device = new Device();
        var userId = Guid.NewGuid();

        device.UserId = userId;

        device.UserId.Should().Be(userId);
    }

    [Fact]
    public void Device_Should_Assign_DeviceName()
    {
        var device = new Device();

        device.DeviceName = "John's iPhone";

        device.DeviceName.Should().Be("John's iPhone");
    }

    [Fact]
    public void Device_Should_Assign_DeviceIdentifier()
    {
        var device = new Device();

        device.DeviceIdentifier = "device-token-123";

        device.DeviceIdentifier.Should().Be("device-token-123");
    }

    [Fact]
    public void Device_Should_Assign_DeviceType()
    {
        var device = new Device();

        device.DeviceType = "Mobile";

        device.DeviceType.Should().Be("Mobile");
    }

    [Fact]
    public void Device_Should_Assign_OperatingSystem()
    {
        var device = new Device();

        device.OperatingSystem = "Android 15";

        device.OperatingSystem.Should().Be("Android 15");
    }

    [Fact]
    public void Device_Should_Assign_AppVersion()
    {
        var device = new Device();

        device.AppVersion = "1.0.0";

        device.AppVersion.Should().Be("1.0.0");
    }

    [Fact]
    public void Device_Should_Assign_IsActive()
    {
        var device = new Device();

        device.IsActive = true;

        device.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Device_Should_Assign_All_Properties()
    {
        var deviceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var device = new Device
        {
            DeviceId = deviceId,
            UserId = userId,
            DeviceIdentifier = "device-token-123",
            DeviceName = "John's iPhone",
            DeviceType = "Mobile",
            OperatingSystem = "Android 15",
            AppVersion = "1.0.0",
            IsActive = true
        };

        device.DeviceId.Should().Be(deviceId);
        device.UserId.Should().Be(userId);
        device.DeviceIdentifier.Should().Be("device-token-123");
        device.DeviceName.Should().Be("John's iPhone");
        device.DeviceType.Should().Be("Mobile");
        device.OperatingSystem.Should().Be("Android 15");
        device.AppVersion.Should().Be("1.0.0");
        device.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Device_Should_Have_Default_Values()
    {
        var device = new Device();

        device.DeviceId.Should().Be(Guid.Empty);
        device.UserId.Should().Be(Guid.Empty);
        device.DeviceName.Should().BeNull();
        device.DeviceIdentifier.Should().BeEmpty();
        device.DeviceType.Should().BeEmpty();
        device.OperatingSystem.Should().BeNull();
        device.AppVersion.Should().BeNull();
        device.IsActive.Should().BeFalse();
    }
}