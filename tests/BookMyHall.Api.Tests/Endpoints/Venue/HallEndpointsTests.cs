using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BookMyHall.Api.Tests;

public sealed class HallEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task GetHalls_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/halls");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/halls/{hallId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/halls/not-a-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateHall_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            hallName = "Grand Celebration Hall",
            hallCategoryId = Guid.NewGuid(),
            cityId = Guid.NewGuid(),
            address = "Main Road",
            capacity = 500,
            price = 25000,
            description = "A spacious event hall"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/halls",request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateHall_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallId = Guid.NewGuid();
        var request = new
        {
            hallName = "Updated Celebration Hall",
            hallCategoryId = Guid.NewGuid(),
            cityId = Guid.NewGuid(),
            address = "Updated Main Road",
            capacity = 600,
            price = 30000,
            description = "Updated event hall",
            isActive = true
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/halls/{hallId}",request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateHall_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            hallName = "Updated Celebration Hall",
            hallCategoryId = Guid.NewGuid(),
            cityId = Guid.NewGuid(),
            address = "Updated Main Road",
            capacity = 600,
            price = 30000,
            description = "Updated event hall",
            isActive = true
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/halls/not-a-guid",request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteHall_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/halls/{hallId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteHall_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/halls/not-a-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}