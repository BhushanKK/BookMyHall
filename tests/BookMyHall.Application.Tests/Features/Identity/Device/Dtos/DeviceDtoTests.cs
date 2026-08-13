using System.Text.Json;
using BookMyHall.Application.Features.Identity;
using FluentAssertions;

namespace BookMyHall.Application.Tests.Features.Identity;

public sealed class DeviceDtoTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Act
        var dto = new DeviceDto();

        // Assert
        dto.DeviceId.Should().Be(Guid.Empty);
        dto.UserId.Should().Be(Guid.Empty);

        dto.DeviceIdentifier.Should().BeEmpty();
        dto.PushNotificationToken.Should().BeEmpty();
        dto.DeviceName.Should().BeEmpty();
        dto.DeviceType.Should().BeEmpty();
        dto.OperatingSystem.Should().BeEmpty();
        dto.Browser.Should().BeEmpty();
        dto.AppVersion.Should().BeEmpty();
        dto.LastIpAddress.Should().BeEmpty();

        dto.LastLoginDate.Should().Be(default);
        dto.LastActivity.Should().BeNull();
        dto.IsTrusted.Should().BeFalse();
        dto.TrustedDate.Should().BeNull();
        dto.IsActive.Should().BeFalse();
        dto.CreatedDate.Should().Be(default);
        dto.UpdatedDate.Should().BeNull();
    }

    [Fact]
    public void Properties_ShouldSetAndGetValues()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lastLoginDate = DateTimeOffset.UtcNow;
        var lastActivity = DateTimeOffset.UtcNow.AddMinutes(-5);
        var trustedDate = DateTimeOffset.UtcNow.AddDays(-1);
        var createdDate = DateTimeOffset.UtcNow.AddDays(-10);
        var updatedDate = DateTimeOffset.UtcNow;

        var dto = new DeviceDto
        {
            DeviceId = deviceId,
            UserId = userId,
            DeviceIdentifier = "device-123",
            PushNotificationToken = "push-token",
            DeviceName = "iPhone",
            DeviceType = "Mobile",
            OperatingSystem = "iOS",
            Browser = "Safari",
            AppVersion = "1.0.0",
            LastIpAddress = "192.168.1.10",
            LastLoginDate = lastLoginDate,
            LastActivity = lastActivity,
            IsTrusted = true,
            TrustedDate = trustedDate,
            IsActive = true,
            CreatedDate = createdDate,
            UpdatedDate = updatedDate
        };

        // Assert
        dto.DeviceId.Should().Be(deviceId);
        dto.UserId.Should().Be(userId);
        dto.DeviceIdentifier.Should().Be("device-123");
        dto.PushNotificationToken.Should().Be("push-token");
        dto.DeviceName.Should().Be("iPhone");
        dto.DeviceType.Should().Be("Mobile");
        dto.OperatingSystem.Should().Be("iOS");
        dto.Browser.Should().Be("Safari");
        dto.AppVersion.Should().Be("1.0.0");
        dto.LastIpAddress.Should().Be("192.168.1.10");
        dto.LastLoginDate.Should().Be(lastLoginDate);
        dto.LastActivity.Should().Be(lastActivity);
        dto.IsTrusted.Should().BeTrue();
        dto.TrustedDate.Should().Be(trustedDate);
        dto.IsActive.Should().BeTrue();
        dto.CreatedDate.Should().Be(createdDate);
        dto.UpdatedDate.Should().Be(updatedDate);
    }

   [Fact]
public void JsonSerialization_ShouldIgnoreDeviceIdAndUserId()
{
    // Arrange
    var dto = new DeviceDto
    {
        DeviceId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        DeviceIdentifier = "device-123",
        DeviceName = "Test Device",
        DeviceType = "Mobile",
        IsActive = true
    };

    // Act
    var json = JsonSerializer.Serialize(dto);
    using var document = JsonDocument.Parse(json);

    var properties = document.RootElement
        .EnumerateObject()
        .Select(property => property.Name)
        .ToList();

    // Assert
    properties.Should().NotContain("DeviceId");
    properties.Should().NotContain("UserId");

    properties.Should().Contain("DeviceIdentifier");
    properties.Should().Contain("DeviceName");
    properties.Should().Contain("DeviceType");
    properties.Should().Contain("IsActive");
}

    [Fact]
    public void JsonSerialization_ShouldIncludePublicProperties()
    {
        // Arrange
        var dto = new DeviceDto
        {
            DeviceIdentifier = "device-123",
            PushNotificationToken = "token-123",
            DeviceName = "Chrome",
            DeviceType = "Desktop",
            OperatingSystem = "Windows",
            Browser = "Chrome",
            AppVersion = "2.0.0",
            LastIpAddress = "127.0.0.1",
            IsTrusted = true,
            IsActive = true
        };

        // Act
        var json = JsonSerializer.Serialize(dto);

        // Assert
        json.Should().Contain("DeviceIdentifier");
        json.Should().Contain("PushNotificationToken");
        json.Should().Contain("DeviceName");
        json.Should().Contain("DeviceType");
        json.Should().Contain("OperatingSystem");
        json.Should().Contain("Browser");
        json.Should().Contain("AppVersion");
        json.Should().Contain("LastIpAddress");
        json.Should().Contain("IsTrusted");
        json.Should().Contain("IsActive");
    }
}