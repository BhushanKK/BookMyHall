using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookMyHall.Api.Tests;

public sealed class DistrictEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task GetDistricts_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/districts");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDistrictById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var districtId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(
            $"/api/districts/{districtId}");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDistrictById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/districts/not-a-guid");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateDistrict_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var request = new
        {
            districtName = "Nashik"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/districts",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateDistrict_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var districtId = Guid.NewGuid();

        var request = new
        {
            districtName = "Updated District"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/districts/{districtId}",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateDistrict_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        var request = new
        {
            districtName = "Updated District"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/districts/not-a-guid",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteDistrict_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var districtId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync(
            $"/api/districts/{districtId}");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteDistrict_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.DeleteAsync(
            "/api/districts/not-a-guid");

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