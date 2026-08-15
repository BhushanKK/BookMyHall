using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookMyHall.Api.Tests;

public sealed class EventCategoryEndpointsTests(
    WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task GetEventCategories_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/event-categories");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetEventCategoryById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var eventCategoryId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(
            $"/api/event-categories/{eventCategoryId}");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetEventCategoryById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/event-categories/not-a-guid");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateEventCategory_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var request = new
        {
            eventCategoryName = "Wedding"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/event-categories",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateEventCategory_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var eventCategoryId = Guid.NewGuid();

        var request = new
        {
            eventCategoryName = "Updated Wedding"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/event-categories/{eventCategoryId}",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateEventCategory_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        var request = new
        {
            eventCategoryName = "Updated Wedding"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/event-categories/not-a-guid",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteEventCategory_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var eventCategoryId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync(
            $"/api/event-categories/{eventCategoryId}");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteEventCategory_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.DeleteAsync(
            "/api/event-categories/not-a-guid");

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