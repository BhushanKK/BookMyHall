using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookMyHall.Api.Tests;

public sealed class FacilityEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task GetFacilities_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/facilities");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetFacilityById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var facilityId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(
            $"/api/facilities/{facilityId}");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetFacilityById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/facilities/not-a-guid");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateFacility_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var request = new
        {
            facilityName = "Parking"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/facilities",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateFacility_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var facilityId = Guid.NewGuid();

        var request = new
        {
            facilityName = "Updated Parking"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/facilities/{facilityId}",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateFacility_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        var request = new
        {
            facilityName = "Updated Parking"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/facilities/not-a-guid",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteFacility_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var facilityId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync(
            $"/api/facilities/{facilityId}");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteFacility_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.DeleteAsync(
            "/api/facilities/not-a-guid");

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