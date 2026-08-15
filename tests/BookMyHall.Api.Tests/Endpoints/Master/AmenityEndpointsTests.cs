using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookMyHall.Api.Tests;

public sealed class AmenityEndpointsTests(
    WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task GetAmenities_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.GetAsync("/api/amenities");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAmenityById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var amenityId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(
            $"/api/amenities/{amenityId}");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAmenityById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/amenities/not-a-guid");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateAmenity_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var request = new
        {
            amenityName = "Parking"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/amenities",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateAmenity_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var amenityId = Guid.NewGuid();

        var request = new
        {
            amenityName = "Updated Parking"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/amenities/{amenityId}",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateAmenity_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        var request = new
        {
            amenityName = "Updated Parking"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/amenities/not-a-guid",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAmenity_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var amenityId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync(
            $"/api/amenities/{amenityId}");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteAmenity_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.DeleteAsync(
            "/api/amenities/not-a-guid");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    // =========================================================
    // Helpers
    // =========================================================

    private HttpClient CreateClient()
    {
        return _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
    }

    private static async Task AssertStatusCodeAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatusCode)
    {
        var responseBody =
            await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(
            expectedStatusCode,
            $"Actual status code: {(int)response.StatusCode} {response.StatusCode}. " +
            $"Response body: {responseBody}");
    }
}