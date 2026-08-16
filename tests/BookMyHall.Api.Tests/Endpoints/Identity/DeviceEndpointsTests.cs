using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

namespace BookMyHall.Api.Tests;

public sealed class DeviceEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task RegisterDevice_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            deviceIdentifier = "device-123",
            deviceName = "Chrome",
            deviceType = "Web",
            operatingSystem = "Windows"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/devices",request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDevices_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/devices");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDeviceById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var deviceId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/devices/{deviceId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDeviceById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/devices/not-a-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDevice_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = Guid.NewGuid();
        var deviceIdentifier = "device-123";
        var request = new
        {
            deviceName = "Updated Device",
            deviceType = "Web",
            operatingSystem = "Windows"
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/devices/{userId}/{deviceIdentifier}",request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateDevice_WithInvalidUserId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            deviceName = "Updated Device",
            deviceType = "Web",
            operatingSystem = "Windows"
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/devices/not-a-guid/device-123",request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteDevice_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();
        var deviceIdentifier = "device-123";

        // Act
        var response = await client.DeleteAsync($"/api/devices/{userId}/{deviceIdentifier}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteDevice_WithInvalidUserId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/devices/not-a-guid/device-123");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}