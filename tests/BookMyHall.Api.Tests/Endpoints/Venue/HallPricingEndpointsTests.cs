using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BookMyHall.Api.Tests;

public sealed class HallPricingEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task GetHallPricings_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/hall-pricings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallPricingById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var hallPricingId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(
            $"/api/hall-pricings/{hallPricingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallPricingById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/hall-pricings/not-a-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateHallPricing_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            hallId = Guid.NewGuid(),
            eventCategoryId = Guid.NewGuid(),
            dayType = 1,
            startTime = "09:00:00",
            endTime = "18:00:00",
            price = 5000,
            isActive = true
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/hall-pricings/",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateHallPricing_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var hallPricingId = Guid.NewGuid();

        var request = new
        {
            hallId = Guid.NewGuid(),
            eventCategoryId = Guid.NewGuid(),
            dayType = 1,
            startTime = "09:00:00",
            endTime = "18:00:00",
            price = 6000,
            isActive = true
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/hall-pricings/{hallPricingId}",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateHallPricing_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            hallId = Guid.NewGuid(),
            eventCategoryId = Guid.NewGuid(),
            dayType = 1,
            startTime = "09:00:00",
            endTime = "18:00:00",
            price = 6000,
            isActive = true
        };

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/hall-pricings/not-a-guid",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteHallPricing_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var hallPricingId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync(
            $"/api/hall-pricings/{hallPricingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteHallPricing_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync(
            "/api/hall-pricings/not-a-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetHallPricingByHallAndEventCategory_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var hallId = Guid.NewGuid();
        var eventCategoryId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(
            $"/api/hall-pricings/hall/{hallId}/category/{eventCategoryId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallPricingByHallAndEventCategory_WithInvalidHallId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        var eventCategoryId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(
            $"/api/hall-pricings/hall/not-a-guid/category/{eventCategoryId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetHallPricingByHallAndEventCategory_WithInvalidEventCategoryId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        var hallId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(
            $"/api/hall-pricings/hall/{hallId}/category/not-a-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}