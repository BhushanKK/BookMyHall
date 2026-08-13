using AutoMapper;
using BookMyHall.Application.Features.Identity;
using FluentAssertions;

using Microsoft.Extensions.Logging;

namespace BookMyHall.Application.Tests.Features.Identity.Device.Mappers;

public sealed class DeviceMappingProfileTests
{
    private readonly IMapper _mapper;

    // public DeviceMappingProfileTests()
    // {
    //     var configuration = new MapperConfiguration(cfg =>
    //     {
    //         cfg.AddProfile<DeviceMappingProfile>();
    //     },null);

    //     configuration.AssertConfigurationIsValid();

    //     _mapper = configuration.CreateMapper();
    // }

     public DeviceMappingProfileTests()
    {
        using var loggerFactory = LoggerFactory.Create(
            builder => { });

        var configuration = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile<DeviceMappingProfile>();
            },
            loggerFactory);

        configuration.AssertConfigurationIsValid();

        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Configuration_ShouldBeValid()
    {
        // Arrange
       using var loggerFactory = LoggerFactory.Create(
            builder => { });

        var configuration = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile<DeviceMappingProfile>();
            },
            loggerFactory);


        // Act & Assert
        configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void Should_MapDeviceToDeviceDto()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lastLoginDate = DateTimeOffset.UtcNow;
        var lastActivity = DateTimeOffset.UtcNow.AddMinutes(-10);
        var trustedDate = DateTimeOffset.UtcNow.AddDays(-1);
        var createdDate = DateTimeOffset.UtcNow.AddDays(-10);
        var updatedDate = DateTimeOffset.UtcNow;

        var device = new BookMyHall.Domain.Identity.Device
        {
            DeviceId = deviceId,
            UserId = userId,
            DeviceIdentifier = "device-123",
            PushNotificationToken = "push-token",
            DeviceName = "Test Device",
            DeviceType = "Mobile",
            OperatingSystem = "Android",
            Browser = "Chrome",
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

        // Act
        var result = _mapper.Map<DeviceDto>(device);

        // Assert
        result.Should().NotBeNull();
        result.DeviceId.Should().Be(deviceId);
        result.UserId.Should().Be(userId);
        result.DeviceIdentifier.Should().Be("device-123");
        result.PushNotificationToken.Should().Be("push-token");
        result.DeviceName.Should().Be("Test Device");
        result.DeviceType.Should().Be("Mobile");
        result.OperatingSystem.Should().Be("Android");
        result.Browser.Should().Be("Chrome");
        result.AppVersion.Should().Be("1.0.0");
        result.LastIpAddress.Should().Be("192.168.1.10");
        result.LastLoginDate.Should().Be(lastLoginDate);
        result.LastActivity.Should().Be(lastActivity);
        result.IsTrusted.Should().BeTrue();
        result.TrustedDate.Should().Be(trustedDate);
        result.IsActive.Should().BeTrue();
        result.CreatedDate.Should().Be(createdDate);
        result.UpdatedDate.Should().Be(updatedDate);
    }

    [Fact]
    public void Should_MapDeviceDtoToDevice()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lastLoginDate = DateTimeOffset.UtcNow;
        var lastActivity = DateTimeOffset.UtcNow.AddMinutes(-10);
        var trustedDate = DateTimeOffset.UtcNow.AddDays(-1);
        var createdDate = DateTimeOffset.UtcNow.AddDays(-10);
        var updatedDate = DateTimeOffset.UtcNow;

        var dto = new DeviceDto
        {
            DeviceId = deviceId,
            UserId = userId,
            DeviceIdentifier = "device-123",
            PushNotificationToken = "push-token",
            DeviceName = "Test Device",
            DeviceType = "Mobile",
            OperatingSystem = "Android",
            Browser = "Chrome",
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

        // Act
        var result = _mapper.Map<BookMyHall.Domain.Identity.Device>(dto);

        // Assert
        result.Should().NotBeNull();
        result.DeviceId.Should().Be(deviceId);
        result.UserId.Should().Be(userId);
        result.DeviceIdentifier.Should().Be("device-123");
        result.PushNotificationToken.Should().Be("push-token");
        result.DeviceName.Should().Be("Test Device");
        result.DeviceType.Should().Be("Mobile");
        result.OperatingSystem.Should().Be("Android");
        result.Browser.Should().Be("Chrome");
        result.AppVersion.Should().Be("1.0.0");
        result.LastIpAddress.Should().Be("192.168.1.10");
        result.LastLoginDate.Should().Be(lastLoginDate);
        result.LastActivity.Should().Be(lastActivity);
        result.IsTrusted.Should().BeTrue();
        result.TrustedDate.Should().Be(trustedDate);
        result.IsActive.Should().BeTrue();
        result.CreatedDate.Should().Be(createdDate);
        result.UpdatedDate.Should().Be(updatedDate);
    }
}