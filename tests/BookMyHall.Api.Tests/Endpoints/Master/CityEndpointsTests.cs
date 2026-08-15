using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookMyHall.Api.Tests;

public sealed class CityEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task GetCities_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        // Act
        var response = await client.GetAsync("/api/cities");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCityById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        var cityId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(
            $"/api/cities/{cityId}");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCityById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        // Act
        var response = await client.GetAsync(
            "/api/cities/not-a-guid");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateCity_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        var request = new
        {
            cityName = "Mumbai"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/cities",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateCity_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        var cityId = Guid.NewGuid();

        var request = new
        {
            cityName = "Updated City"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/cities/{cityId}",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateCity_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        var request = new
        {
            cityName = "Updated City"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/cities/not-a-guid",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCity_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        var cityId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync(
            $"/api/cities/{cityId}");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteCity_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        // Act
        var response = await client.DeleteAsync(
            "/api/cities/not-a-guid");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    private static async Task AssertStatusCodeAsync(
        HttpResponseMessage response,
        HttpStatusCode expected)
    {
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(
            expected,
            $"Actual status: {(int)response.StatusCode} {response.StatusCode}. " +
            $"Response body: {body}");
    }
}