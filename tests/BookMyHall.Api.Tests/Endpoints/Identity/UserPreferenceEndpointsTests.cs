using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BookMyHall.Api.Tests;

public sealed class UserPreferenceEndpointsTests(
    BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task GetUserPreference_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/user-preferences/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserPreference_WithInvalidPreferenceIdRoute_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/user-preferences/not-a-guid/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserPreference_WithInvalidUserIdRoute_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/user-preferences/{userId}/not-a-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserPreference_WithBothInvalidRouteIds_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/user-preferences/not-a-guid/not-a-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    // =========================================================
    // POST /api/user-preferences/
    // =========================================================

    [Fact]
    public async Task CreateUserPreference_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            currencyCode = "INR",
            timeZone = "Asia/Kolkata",
            dateFormat = "DD-MM-YYYY",
            timeFormat = "24",
            languageCode = "en-IN",
            emailNotification = true,
            smsNotification = false,
            pushNotification = true,
            theme = "Light"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/user-preferences/", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
