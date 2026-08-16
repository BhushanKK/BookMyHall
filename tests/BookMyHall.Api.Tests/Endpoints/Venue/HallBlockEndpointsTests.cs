using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BookMyHall.Api.Tests;

public sealed class HallBlockEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task GetHallBlocks_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/hall-blocks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallBlockById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallBlockId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/hall-blocks/{hallBlockId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallBlockById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/hall-blocks/not-a-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateHallBlock_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            hallId = Guid.NewGuid(),
            blockFromDate = "2026-08-20",
            blockToDate = "2026-08-22",
            startTime = "10:00:00",
            endTime = "18:00:00",
            reason = "Maintenance"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/hall-blocks",request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateHallBlock_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallBlockId = Guid.NewGuid();

        var request = new
        {
            hallId = Guid.NewGuid(),
            blockFromDate = "2026-08-20",
            blockToDate = "2026-08-22",
            startTime = "10:00:00",
            endTime = "18:00:00",
            reason = "Updated maintenance",
            isActive = true
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/hall-blocks/{hallBlockId}",request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateHallBlock_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            hallId = Guid.NewGuid(),
            blockFromDate = "2026-08-20",
            blockToDate = "2026-08-22",
            startTime = "10:00:00",
            endTime = "18:00:00",
            reason = "Updated maintenance",
            isActive = true
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/hall-blocks/not-a-guid",request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteHallBlock_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallBlockId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/hall-blocks/{hallBlockId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteHallBlock_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/hall-blocks/not-a-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

   

    [Fact]
    public async Task GetHallBlocksByHallId_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/halls/not-a-guid/blocks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}